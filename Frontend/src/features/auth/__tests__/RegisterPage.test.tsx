import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import RegisterPage from "../RegisterPage";
import { AuthProvider } from "@/features/auth/AuthProvider";
import { Router } from "react-router-dom";
import { createMemoryHistory } from "history";
import { vi, describe, beforeEach, expect } from "vitest";

const API_URL = "http://localhost";

describe("RegisterPage", () => {
  beforeEach(() => {
    localStorage.clear();
    vi.clearAllMocks();
  });

  test("redirige vers /confirm-email-sent après un register réussi", async () => {
    const history = createMemoryHistory({ initialEntries: ["/register"] });

    global.fetch = vi.fn((input: RequestInfo) => {
      const url = input.toString();
      if (url === `${API_URL}/api/v1/auth/register`) {
        return Promise.resolve(
          new Response(
            JSON.stringify({
              id: "1",
              email: "new@example.com",
              displayName: "Gaetan"
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
        <Router location={history.location} navigator={history}>
          <RegisterPage />
        </Router>
      </AuthProvider>
    );

    fireEvent.change(screen.getByLabelText("Email"), {
      target: { value: "new@example.com" }
    });

    fireEvent.change(screen.getByLabelText("Nom affiché"), {
      target: { value: "Gaetan" }
    });

    fireEvent.change(screen.getByLabelText("Mot de passe"), {
      target: { value: "Abcd1234!" }
    });

    fireEvent.change(screen.getByLabelText("Confirmer le mot de passe"), {
      target: { value: "Abcd1234!" }
    });

    fireEvent.click(screen.getByRole("button", { name: "Créer le compte" }));

    await waitFor(() => {
      expect(history.location.pathname).toBe("/confirm-email-sent");
    });
  });

  test("affiche une erreur si email déjà utilisé (409)", async () => {
    global.fetch = vi.fn((input: RequestInfo) => {
      const url = input.toString();
      if (url === `${API_URL}/api/v1/auth/register`) {
        return Promise.resolve(
          new Response(
            JSON.stringify({ error: "Email already in use." }),
            {
              status: 409,
              headers: { "Content-Type": "application/json" }
            }
          )
        );
      }
      throw new Error("URL non mockée: " + url);
    }) as unknown as typeof fetch;

    const history = createMemoryHistory({ initialEntries: ["/register"] });

    render(
      <AuthProvider>
        <Router location={history.location} navigator={history}>
          <RegisterPage />
        </Router>
      </AuthProvider>
    );

    fireEvent.change(screen.getByLabelText("Email"), {
      target: { value: "exists@example.com" }
    });

    fireEvent.change(screen.getByLabelText("Nom affiché"), {
      target: { value: "Gaetan" }
    });

    fireEvent.change(screen.getByLabelText("Mot de passe"), {
      target: { value: "Abcd1234!" }
    });

    fireEvent.change(screen.getByLabelText("Confirmer le mot de passe"), {
      target: { value: "Abcd1234!" }
    });

    fireEvent.click(screen.getByRole("button", { name: "Créer le compte" }));

    await waitFor(() => {
      expect(
        screen.getByText("Cet email est déjà utilisé.")
      ).toBeInTheDocument();
    });
  });

  test("affiche une erreur si les mots de passe ne correspondent pas", async () => {
    const history = createMemoryHistory({ initialEntries: ["/register"] });

    render(
      <AuthProvider>
        <Router location={history.location} navigator={history}>
          <RegisterPage />
        </Router>
      </AuthProvider>
    );

    fireEvent.change(screen.getByLabelText("Email"), {
      target: { value: "new@example.com" }
    });

    fireEvent.change(screen.getByLabelText("Nom affiché"), {
      target: { value: "Gaetan" }
    });

    fireEvent.change(screen.getByLabelText("Mot de passe"), {
      target: { value: "Abcd1234!" }
    });

    fireEvent.change(screen.getByLabelText("Confirmer le mot de passe"), {
      target: { value: "Abcd1234?" }
    });

    fireEvent.click(screen.getByRole("button", { name: "Créer le compte" }));

    await waitFor(() => {
      expect(
        screen.getByText("Les mots de passe ne correspondent pas.")
      ).toBeInTheDocument();
    });
  });

  test("valide le mot de passe côté client (Standard SaaS)", async () => {
    const history = createMemoryHistory({ initialEntries: ["/register"] });

    render(
      <AuthProvider>
        <Router location={history.location} navigator={history}>
          <RegisterPage />
        </Router>
      </AuthProvider>
    );

    fireEvent.change(screen.getByLabelText("Email"), {
      target: { value: "new@example.com" }
    });

    fireEvent.change(screen.getByLabelText("Nom affiché"), {
      target: { value: "Gaetan" }
    });

    fireEvent.change(screen.getByLabelText("Mot de passe"), {
      target: { value: "weak" }
    });

    fireEvent.change(screen.getByLabelText("Confirmer le mot de passe"), {
      target: { value: "weak" }
    });

    fireEvent.click(screen.getByRole("button", { name: "Créer le compte" }));

    await waitFor(() => {
      expect(
        screen.getByText(/le mot de passe doit contenir 8 caractères/i)
      ).toBeInTheDocument();
    });
  });
});
