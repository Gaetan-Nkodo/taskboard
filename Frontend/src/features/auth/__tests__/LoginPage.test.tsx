import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import LoginPage from "../LoginPage";
import { AuthProvider } from "@/features/auth/AuthProvider";
import { MemoryRouter } from "react-router-dom";
import { vi, describe, beforeEach, expect } from "vitest";

const API_URL = "http://localhost";

describe("LoginPage", () => {
  beforeEach(() => {
    localStorage.clear();
    vi.clearAllMocks();
  });

  test("stocke accessToken + refreshToken + user et navigue", async () => {
    global.fetch = vi.fn((input: RequestInfo) => {
      const url = input.toString();

      if (url === `${API_URL}/api/v1/auth/login`) {
        return Promise.resolve(
          new Response(
            JSON.stringify({
              accessToken: "abc",
              refreshToken: "ref",
              user: { id: "1", email: "test@test.com" }
            }),
            {
              status: 200,
              headers: { "Content-Type": "application/json" }
            }
          )
        );
      }

      throw new Error("URL non mockée: " + url);
    }) as unknown as typeof fetch;

    render(
      <AuthProvider>
        <MemoryRouter>
          <LoginPage />
        </MemoryRouter>
      </AuthProvider>
    );

    fireEvent.change(screen.getByLabelText("Email"), {
      target: { value: "test@test.com" }
    });

    fireEvent.change(screen.getByLabelText("Mot de passe"), {
      target: { value: "123456" }
    });

    fireEvent.click(screen.getByRole("button", { name: "Se connecter" }));

    await waitFor(() => {
      expect(localStorage.getItem("accessToken")).toBe("abc");
      expect(localStorage.getItem("refreshToken")).toBe("ref");
      expect(JSON.parse(localStorage.getItem("user")!)).toEqual({
        id: "1",
        email: "test@test.com"
      });
    });
  });

  test("affiche une erreur si login échoue", async () => {
    global.fetch = vi.fn((input: RequestInfo) => {
      const url = input.toString();

      if (url === `${API_URL}/api/v1/auth/login`) {
        return Promise.resolve(
          new Response("Erreur", {
            status: 400,
            headers: { "Content-Type": "text/plain" }
          })
        );
      }

      throw new Error("URL non mockée: " + url);
    }) as unknown as typeof fetch;

    render(
      <AuthProvider>
        <MemoryRouter>
          <LoginPage />
        </MemoryRouter>
      </AuthProvider>
    );

    // 🔥 IMPORTANT : remplir les champs sinon le formulaire NE SE SOUMET PAS
    fireEvent.change(screen.getByLabelText("Email"), {
      target: { value: "x@test.com" }
    });

    fireEvent.change(screen.getByLabelText("Mot de passe"), {
      target: { value: "123" }
    });

    fireEvent.click(screen.getByRole("button", { name: "Se connecter" }));

    await waitFor(() => {
      expect(screen.getByText("Erreur")).toBeInTheDocument();
    });
  });
});
