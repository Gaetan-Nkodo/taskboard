import { describe, it, expect } from "vitest";
import { render, waitFor } from "@testing-library/react";
import { ProtectedRoute }  from "../ProtectedRoute";
import { AuthProvider } from "../AuthProvider";
import { MemoryRouter } from "react-router-dom";

describe("ProtectedRoute", () => {
  it("redirige vers /login si non connecté", async () => {
    const { container } = render(
      <MemoryRouter initialEntries={["/"]}>
        <AuthProvider>
          <ProtectedRoute>
            <div>PRIVATE</div>
          </ProtectedRoute>
        </AuthProvider>
      </MemoryRouter>
    );

    await waitFor(() => {});

    expect(container.innerHTML).not.toContain("PRIVATE");
  });

  it("rend les children si connecté", async () => {
    localStorage.setItem("user", JSON.stringify({
      id: "1",
      email: "test@test.com",
      name: "Gaetan"
    }));
    localStorage.setItem("accessToken", "abc");
    localStorage.setItem("refreshToken", "ref");

    const { container } = render(
      <MemoryRouter initialEntries={["/"]}>
        <AuthProvider>
          <ProtectedRoute>
            <div>PRIVATE</div>
          </ProtectedRoute>
        </AuthProvider>
      </MemoryRouter>
    );

    await waitFor(() => {
      expect(container.innerHTML).toContain("PRIVATE");
    });
  });
});
