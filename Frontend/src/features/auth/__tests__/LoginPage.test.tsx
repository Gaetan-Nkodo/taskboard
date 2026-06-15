import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, fireEvent, screen, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import LoginPage from "../LoginPage";
import { AuthProvider } from "@/features/auth/AuthProvider";

// MOCK useAuth()
const loginMock = vi.fn();

vi.mock("@/features/auth/useAuth", () => ({
  useAuth: () => ({
    login: loginMock
  })
}));

// MOCK useNavigate
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

    await waitFor(() => {
      expect(navigateMock).toHaveBeenCalled();
    });

    expect(JSON.parse(localStorage.getItem("user")!)).toEqual({
      id: "1",
      email: "test@test.com",
      displayName: "Test User"
    });
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

    expect(await screen.findByText("Identifiants invalides")).toBeInTheDocument();
  });
});
