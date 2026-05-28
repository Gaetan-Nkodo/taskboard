import { describe, it, expect, vi } from "vitest";
import { render, fireEvent, waitFor } from "@testing-library/react";
import LoginPage from "../LoginPage";
import { AuthProvider } from "../AuthProvider";
import { MemoryRouter } from "react-router-dom";

const navigateMock = vi.fn();

vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return {
    ...actual,
    useNavigate: () => navigateMock,
    useLocation: () => ({ state: { from: "/boards/123" } })
  };
});

describe("returnTo", () => {
  it("redirige vers la page d’origine après login", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      json: async () => ({
        token: "abc",
        user: { id: "1", email: "test@test.com", name: "Gaetan" }
      })
    } as any);

    const { getByPlaceholderText, getByText } = render(
      <MemoryRouter>
        <AuthProvider>
          <LoginPage />
        </AuthProvider>
      </MemoryRouter>
    );

    fireEvent.change(getByPlaceholderText("Email"), {
      target: { value: "test@test.com" }
    });

    fireEvent.change(getByPlaceholderText("Mot de passe"), {
      target: { value: "123" }
    });

    fireEvent.click(getByText("Se connecter"));

    await waitFor(() => {
      expect(navigateMock).toHaveBeenCalledWith("/boards/123");
    });
  });
});
