import { render, screen } from "@testing-library/react";
import ConfirmEmailSentPage from "../ConfirmEmailSentPage";
import { MemoryRouter } from "react-router-dom";
import { describe, expect, it } from "vitest";

describe("ConfirmEmailSentPage", () => {
  it("affiche le message et le lien vers login", () => {
    render(
      <MemoryRouter initialEntries={["/confirm-email-sent"]}>
        <ConfirmEmailSentPage />
      </MemoryRouter>
    );

    expect(
      screen.getByText(/un lien de confirmation vous a été envoyé/i)
    ).toBeInTheDocument();

    expect(
      screen.getByRole("link", { name: /retour à la connexion/i })
    ).toBeInTheDocument();
  });
});
