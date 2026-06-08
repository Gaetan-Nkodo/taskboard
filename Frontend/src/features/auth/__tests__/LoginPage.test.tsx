import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, fireEvent, screen, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { AuthProvider } from "../AuthProvider";
import LoginPage from "../LoginPage";

// --- MOCK useAuth() ---
const loginMock = vi.fn();

vi.mock("../useAuth", () => ({
  useAuth: () => ({
    login: loginMock
  })
}));

// --- MOCK useNavigate() ---
const navigateMock = vi.fn();

vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return {
    ...actual,
    useNavigate: () => navigateMock,
    useLocation: () => ({ state: null })
  };
});

describe("LoginPage", () => {
  beforeEach(() => {
    localStorage.clear();
    vi.clearAllMocks();
  });

  it("affiche le message d’expiration si logoutReason = expired", () => {
    localStorage.setItem("logoutReason", "expired");

    render(
      <MemoryRouter>
        <AuthProvider>
          <LoginPage />
        </AuthProvider>
      </MemoryRouter>
    );

    expect(
      screen.getByText("Votre session a expiré, veuillez vous reconnecter")
    ).toBeInTheDocument();

    // logoutReason doit être supprimé
    expect(localStorage.getItem("logoutReason")).toBe(null);
  });

  it("stocke accessToken + refreshToken + user et navigue", async () => {
    loginMock.mockResolvedValue({
      accessToken: "ACCESS_TOKEN",
      refreshToken: "REFRESH_TOKEN",
      user: {
        id: "1",
        email: "test@test.com",
        displayName: "Test User"
      }
    });

    render(
      <MemoryRouter>
        <AuthProvider>
          <LoginPage />
        </AuthProvider>
      </MemoryRouter>
    );

    fireEvent.change(screen.getByPlaceholderText("Email"), {
      target: { value: "test@test.com" }
    });

    fireEvent.change(screen.getByPlaceholderText("Mot de passe"), {
      target: { value: "pwd" }
    });

    fireEvent.submit(screen.getByTestId("login-form"));

    // Navigation OK
    await waitFor(() => {
      expect(navigateMock).toHaveBeenCalled();
    });

    // Vérification localStorage
    expect(JSON.parse(localStorage.getItem("user")!)).toEqual({
      id: "1",
      email: "test@test.com",
      displayName: "Test User"
    });

    expect(localStorage.getItem("accessToken")).toBe("ACCESS_TOKEN");
    expect(localStorage.getItem("refreshToken")).toBe("REFRESH_TOKEN");
  });

  it("affiche une erreur si login échoue", async () => {
    loginMock.mockRejectedValue(new Error("Invalid credentials"));

    render(
      <MemoryRouter>
        <AuthProvider>
          <LoginPage />
        </AuthProvider>
      </MemoryRouter>
    );

    fireEvent.change(screen.getByPlaceholderText("Email"), {
      target: { value: "test@test.com" }
    });

    fireEvent.change(screen.getByPlaceholderText("Mot de passe"), {
      target: { value: "pwd" }
    });

    fireEvent.submit(screen.getByTestId("login-form"));

    expect(await screen.findByText("Invalid credentials")).toBeInTheDocument();
  });
});
