import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, fireEvent, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import ResetPasswordPage from "../ResetPasswordPage";
import { TestProviders } from "@/tests/TestProviders";

// Mock navigation pour éviter un vrai redirect
const mockAssign = vi.fn();
Object.defineProperty(window, "location", {
  value: { assign: mockAssign },
  writable: true
});

describe("ResetPasswordPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  function renderWithToken(token: string = "VALID") {
    return render(
      <MemoryRouter initialEntries={[`/reset-password?token=${token}`]}>
        <TestProviders>
          <ResetPasswordPage />
        </TestProviders>
      </MemoryRouter>
    );
  }

  it("réinitialise le mot de passe avec succès", async () => {
    renderWithToken();

    fireEvent.change(screen.getByPlaceholderText("Nouveau mot de passe"), {
      target: { value: "NewP@ssw0rd!" }
    });

    fireEvent.click(screen.getByText("Mettre à jour"));

    expect(await screen.findByText(/Mot de passe mis à jour/i)).toBeInTheDocument();
  });

  it("affiche une erreur si token invalide", async () => {
    renderWithToken("BADTOKEN");

    fireEvent.change(screen.getByPlaceholderText("Nouveau mot de passe"), {
      target: { value: "NewP@ssw0rd!" }
    });

    fireEvent.click(screen.getByText("Mettre à jour"));

    expect(
      await screen.findByText(/Lien invalide ou expiré/i)
    ).toBeInTheDocument();

    expect(mockAssign).not.toHaveBeenCalled();
  });
});
