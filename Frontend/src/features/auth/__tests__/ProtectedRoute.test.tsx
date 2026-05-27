import { describe, it, expect } from "vitest";
import { render } from "@testing-library/react";
import ProtectedRoute from "../ProtectedRoute";
import { AuthProvider } from "../AuthContext";
import { MemoryRouter } from "react-router-dom";

describe("ProtectedRoute", () => {
  it("redirige vers /login si non connecté", () => {
    const { container } = render(
      <MemoryRouter initialEntries={["/"]}>
        <AuthProvider>
          <ProtectedRoute>
            <div>PRIVATE</div>
          </ProtectedRoute>
        </AuthProvider>
      </MemoryRouter>
    );

    expect(container.innerHTML).not.toContain("PRIVATE");
  });

  it("rend les children si connecté", () => {
    localStorage.setItem("user", JSON.stringify({
      id: "1",
      email: "test@test.com",
      name: "Gaetan"
    }));

    const { container } = render(
      <MemoryRouter initialEntries={["/"]}>
        <AuthProvider>
          <ProtectedRoute>
            <div>PRIVATE</div>
          </ProtectedRoute>
        </AuthProvider>
      </MemoryRouter>
    );

    expect(container.innerHTML).toContain("PRIVATE");
  });
});
