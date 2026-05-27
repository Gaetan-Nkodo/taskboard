import { describe, it, expect, vi } from "vitest";
import { render, fireEvent } from "@testing-library/react";
import LoginPage from "../LoginPage";
import { AuthProvider } from "../AuthContext";
import { MemoryRouter } from "react-router-dom";

describe("LoginPage", () => {
  it("affiche une erreur si login échoue", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: false,
      text: async () => "Invalid credentials"
    } as any);

    const { getByPlaceholderText, getByText, findByText } = render(
      <MemoryRouter>
        <AuthProvider>
          <LoginPage />
        </AuthProvider>
      </MemoryRouter>
    );

    fireEvent.change(getByPlaceholderText("Email"), {
      target: { value: "wrong" }
    });

    fireEvent.change(getByPlaceholderText("Mot de passe"), {
      target: { value: "wrong" }
    });

    fireEvent.click(getByText("Se connecter"));

    expect(await findByText("Invalid credentials")).toBeTruthy();
  });
});
