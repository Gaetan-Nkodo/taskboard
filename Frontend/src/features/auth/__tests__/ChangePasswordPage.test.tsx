import { render, screen, fireEvent } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { ChangePasswordPage } from "../ChangePasswordPage";
import { TestProviders } from "@/tests/test-utils";

beforeEach(() => {
  localStorage.setItem("token", "FAKE_TOKEN");
  localStorage.setItem(
    "user",
    JSON.stringify({
      id: "1",
      email: "test@test.com",
      displayName: "Gaétan"
    })
  );
});

test("affiche un toast de succès quand le mot de passe est correct", async () => {
  render(
    <MemoryRouter>
      <TestProviders>
        <ChangePasswordPage />
      </TestProviders>
    </MemoryRouter>
  );

  fireEvent.change(screen.getByPlaceholderText(/mot de passe actuel/i), {
    target: { value: "OLD" }
  });

  fireEvent.change(screen.getByPlaceholderText(/nouveau mot de passe/i), {
    target: { value: "NEW" }
  });

  fireEvent.change(screen.getByPlaceholderText(/confirmer/i), {
    target: { value: "NEW" }
  });

  fireEvent.click(screen.getByText(/mettre à jour/i));

  expect(
    await screen.findByText(/mot de passe mis à jour/i)
  ).toBeInTheDocument();
});
