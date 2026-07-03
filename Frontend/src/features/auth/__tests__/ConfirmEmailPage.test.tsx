import { render, screen, waitFor } from "@testing-library/react";
import ConfirmEmailPage from "../ConfirmEmailPage";
import { MemoryRouter } from "react-router-dom";
import { vi, describe, beforeEach, expect } from "vitest";
import { AuthProvider } from "@/features/auth/AuthProvider";

const API_URL = "http://localhost";

describe("ConfirmEmailPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  test("affiche succès si token valide", async () => {
    global.fetch = vi.fn((input: RequestInfo) => {
      const url = input.toString();
      if (url.startsWith(`${API_URL}/api/v1/auth/confirm-email`)) {
        return Promise.resolve(new Response("", { status: 200 }));
      }
      throw new Error("URL non mockée: " + url);
    }) as unknown as typeof fetch;

    render(
      <AuthProvider>
        <MemoryRouter initialEntries={["/confirm-email?token=valid"]}>
          <ConfirmEmailPage />
        </MemoryRouter>
      </AuthProvider>
    );

    await waitFor(() => {
      expect(screen.getByText(/email confirmé/i)).toBeInTheDocument();
    });

    expect(
      screen.getByRole("link", { name: /se connecter/i })
    ).toBeInTheDocument();
  });

  test("affiche erreur si token invalide", async () => {
    global.fetch = vi.fn((input: RequestInfo) => {
      const url = input.toString();
      if (url.startsWith(`${API_URL}/api/v1/auth/confirm-email`)) {
        return Promise.resolve(new Response("Invalid", { status: 400 }));
      }
      throw new Error("URL non mockée: " + url);
    }) as unknown as typeof fetch;

    render(
      <AuthProvider>
        <MemoryRouter initialEntries={["/confirm-email?token=invalid"]}>
          <ConfirmEmailPage />
        </MemoryRouter>
      </AuthProvider>
    );

    await waitFor(() => {
      expect(screen.getByText(/lien invalide ou expiré/i)).toBeInTheDocument();
    });

    expect(
      screen.getByRole("link", { name: /créer un compte/i })
    ).toBeInTheDocument();
  });
});
