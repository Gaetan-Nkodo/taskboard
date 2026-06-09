import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, fireEvent, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import ForgotPasswordPage from "../ForgotPasswordPage";

describe("ForgotPasswordPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("envoie l’email et affiche la confirmation", async () => {
    render(
      <MemoryRouter>
        <ForgotPasswordPage />
      </MemoryRouter>
    );

    fireEvent.change(screen.getByPlaceholderText("Votre email"), {
      target: { value: "test@example.com" }
    });

    fireEvent.click(screen.getByText("Envoyer le lien"));

    expect(
      await screen.findByText(/un lien de réinitialisation/i)
    ).toBeInTheDocument();
  });
});
