import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, fireEvent, screen } from "@testing-library/react";

// 1) Mock AuthService AVANT TOUT, avec le BON chemin
vi.mock("../../../core/services/AuthService");

// 2) Mock router APRÈS
const navigateMock = vi.fn();
vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return {
    ...actual,
    useNavigate: () => navigateMock,
    useLocation: () => ({ state: null })
  };
});

// 3) Import APRES les mocks, avec le BON chemin
import { AuthService } from "../../../core/services/AuthService";
import LoginPage from "../LoginPage";
import { AuthProvider } from "../AuthProvider";
import { MemoryRouter } from "react-router-dom";

describe("LoginPage", () => {
  beforeEach(() => {
    localStorage.clear();
    vi.clearAllMocks();
  });

  it("affiche une erreur si login échoue", async () => {
    // Mock effectif
    (AuthService.login as any) = vi.fn().mockRejectedValue(
      new Error("Invalid credentials")
    );

    render(
      <MemoryRouter>
        <AuthProvider>
          <LoginPage />
        </AuthProvider>
      </MemoryRouter>
    );

    // Remplir les champs
    fireEvent.change(screen.getByPlaceholderText("Email"), {
      target: { value: "wrong" }
    });

    fireEvent.change(screen.getByPlaceholderText("Mot de passe"), {
      target: { value: "wrong" }
    });

    // Soumettre le formulaire correctement
    fireEvent.submit(screen.getByRole("form"));

    // Vérifier l'erreur
    expect(await screen.findByText("Invalid credentials")).toBeInTheDocument();

    // Vérifier qu'il n'y a pas eu de navigation
    expect(navigateMock).not.toHaveBeenCalled();
  });
});
