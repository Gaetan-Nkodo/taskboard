import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { AuthProvider } from "@/features/auth/AuthProvider";
import { BoardsPage } from "../BoardsPage";

// 1. Mock du service
const mockGetAll = vi.fn();

vi.mock("@/@/core/services/BoardService", () => ({
  useBoardService: () => ({
    getAll: mockGetAll
  })
}));

describe("BoardsPage", () => {
  beforeEach(() => {
    localStorage.clear();
    vi.restoreAllMocks();
    mockGetAll.mockReset();
  });

  it("affiche les boards après chargement", async () => {
    localStorage.setItem("user", JSON.stringify({ id: "1", email: "test@test.com" }));
    localStorage.setItem("token", "abc123");

    mockGetAll.mockResolvedValue([
      { id: "b1", name: "Board A", description: "Desc A" },
      { id: "b2", name: "Board B", description: "Desc B" }
    ]);

    render(
      <MemoryRouter>
        <AuthProvider>
          <BoardsPage />
        </AuthProvider>
      </MemoryRouter>
    );

    expect(await screen.findByText("Board A")).toBeInTheDocument();
    expect(await screen.findByText("Board B")).toBeInTheDocument();
  });

  it("n'appelle pas l'API si non connecté", async () => {
    render(
      <MemoryRouter>
        <AuthProvider>
          <BoardsPage />
        </AuthProvider>
      </MemoryRouter>
    );

    expect(mockGetAll).not.toHaveBeenCalled();
  });
});
