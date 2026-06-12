import { render, screen, fireEvent } from "@testing-library/react";
import { RouterProvider } from "react-router-dom";
import { router } from "../../app/routes";
import { TestProviders } from "../../tests/test-utils";

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

test("navigation → ProtectedRoute → ChangePasswordPage → succès", async () => {
  router.navigate("/change-password");

  render(
    <TestProviders>
      <RouterProvider router={router} />
    </TestProviders>
  );

  expect(
    await screen.findByText(/changer le mot de passe/i)
  ).toBeInTheDocument();

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
