import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, fireEvent, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { AuthProvider, useAuthContext } from "../AuthProvider";
import LoginPage from "../LoginPage";

const navigateMock = vi.fn();

vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return {
    ...actual,
    useNavigate: () => navigateMock,
    useLocation: () => ({ state: null })
  };
});

function TestConsumer() {
  const ctx = useAuthContext();
  return <div data-testid="ctx">{JSON.stringify(ctx)}</div>;
}

describe("LoginPage", () => {
  beforeEach(() => {
    localStorage.clear();
    vi.clearAllMocks();
  });

  it("stocke accessToken + refreshToken + userDto et navigue", async () => {
    // 🔥 MOCK FETCH (pas AuthService)
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      json: async () => ({
        accessToken: "ACCESS_TOKEN",
        refreshToken: "REFRESH_TOKEN",
        user: {
          id: "1",
          email: "test@test.com",
          displayName: "Test User"
        }
      })
    } as any);

    render(
      <MemoryRouter>
        <AuthProvider>
          <LoginPage />
          <TestConsumer />
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

    const ctx = JSON.parse((await screen.findByTestId("ctx")).textContent!);

    expect(navigateMock).toHaveBeenCalled();
    expect(ctx.accessToken).toBe("ACCESS_TOKEN");
    expect(ctx.refreshToken).toBe("REFRESH_TOKEN");
    expect(ctx.user.email).toBe("test@test.com");
    expect(ctx.user.displayName).toBe("Test User");

    const saved = JSON.parse(localStorage.getItem("user")!);
    expect(saved.displayName).toBe("Test User");
    expect(localStorage.getItem("token")).toBe("ACCESS_TOKEN");
  });
});
