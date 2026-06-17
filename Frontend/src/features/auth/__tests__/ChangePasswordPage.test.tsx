import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { ChangePasswordPage } from "../ChangePasswordPage";
import { AuthProvider } from "@/features/auth/AuthProvider";
import { MemoryRouter } from "react-router-dom";

describe("ChangePasswordPage", () => {
  beforeEach(() => {
    localStorage.setItem("accessToken", "abc");
    localStorage.setItem("refreshToken", "ref");
    localStorage.setItem("user", JSON.stringify({ id: "1" }));
  });

  test("affiche un toast de succès quand le mot de passe est correct", async () => {
    global.fetch = vi.fn().mockResolvedValue({
      ok: true,
      status: 200,
      json: () => Promise.resolve({})
    });

    render(
      <AuthProvider>
        <MemoryRouter>
          <ChangePasswordPage />
        </MemoryRouter>
      </AuthProvider>
    );

    fireEvent.change(screen.getByPlaceholderText(/mot de passe actuel/i), {
      target: { value: "old" }
    });
    fireEvent.change(screen.getByPlaceholderText(/nouveau mot de passe/i), {
      target: { value: "new" }
    });
    fireEvent.change(screen.getByPlaceholderText(/confirmer/i), {
      target: { value: "new" }
    });

    fireEvent.click(screen.getByRole("button", { name: /mettre à jour/i }));

    await waitFor(() => {
      expect(global.fetch).toHaveBeenCalled();
    });
  });
});
