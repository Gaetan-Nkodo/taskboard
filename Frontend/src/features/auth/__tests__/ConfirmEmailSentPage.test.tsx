import { render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { AuthProvider } from "@/features/auth/AuthProvider";
import ConfirmEmailSentPage from "../ConfirmEmailSentPage";

describe("ConfirmEmailSentPage", () => {
  test("affiche le message et le lien vers login", () => {
    render(
      <AuthProvider>
        <MemoryRouter>
          <ConfirmEmailSentPage />
        </MemoryRouter>
      </AuthProvider>
    );

    expect(
      screen.getByText(/un lien de confirmation vous a été envoyé/i)
    ).toBeInTheDocument();

    expect(
      screen.getByRole("link", { name: /retour à la connexion/i })
    ).toBeInTheDocument();
  });
});
