import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { AuthProvider } from "../../auth/AuthProvider";
import { BoardService } from "../../../core/services/BoardService";
import { BoardsPage } from "../BoardsPage";
import type { Board } from "../../../core/models/Board";

describe("BoardsPage", () => {
  beforeEach(() => {
    localStorage.clear();
    vi.restoreAllMocks();
  });

  it("affiche les boards après chargement", async () => {
    localStorage.setItem("user", JSON.stringify({ id: "1", email: "test@test.com" }));
    localStorage.setItem("token", "abc123");

    vi.spyOn(BoardService, "getAll").mockResolvedValue([
      { id: "b1", name: "Board A", description: "Desc A" },
      { id: "b2", name: "Board B", description: "Desc B" }
    ] satisfies Board[]);

    render(
      <MemoryRouter>
        <AuthProvider>
          <BoardsPage />
        </AuthProvider>
      </MemoryRouter>
    );

    await waitFor(() => {
      expect(screen.getByText("Board A")).toBeInTheDocument();
      expect(screen.getByText("Board B")).toBeInTheDocument();
    });
  });

  it("n'appelle pas l'API si non connecté", async () => {
    const spy = vi.spyOn(BoardService, "getAll");

    render(
      <MemoryRouter>
        <AuthProvider>
          <BoardsPage />
        </AuthProvider>
      </MemoryRouter>
    );

    await waitFor(() => {});
    expect(spy).not.toHaveBeenCalled();
  });
});
