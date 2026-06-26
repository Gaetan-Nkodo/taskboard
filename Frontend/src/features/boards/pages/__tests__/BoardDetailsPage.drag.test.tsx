// ------------------------------------------------------
// ⚠️ Les mocks DOIVENT être déclarés AVANT l'import
// ------------------------------------------------------
import { vi } from "vitest";

// Objet STABLE — ne change jamais
const boardMock = {
  id: "1",
  name: "Board A",
  description: "Desc A",
  columns: [
    { id: "backlog", name: "Backlog", order: 1 },
    { id: "ready", name: "Ready", order: 2 },
  ],
};

// Auth
vi.mock("@/features/auth/AuthProvider", () => ({
  AuthProvider: ({ children }: any) => <>{children}</>,
  useAuthContext: () => ({
    accessToken: "abc123",
    user: { id: "1", email: "test@test.com" },
  }),
}));

// BoardService — renvoie TOUJOURS la même référence
const stableBoardService = {
  getBoard: vi.fn().mockResolvedValue(boardMock),
};

vi.mock("@/features/boards/services/BoardService", () => ({
  useBoardService: () => stableBoardService,
}));

// TaskService — renvoie TOUJOURS la même référence
const mockMoveTask = vi.fn();

const stableTaskService = {
  getTasks: vi.fn().mockResolvedValue([
    { id: "t1", name: "Task 1", columnId: "backlog", order: 0 },
    { id: "t2", name: "Task 2", columnId: "backlog", order: 1 },
  ]),
  createTask: vi.fn(),
  updateTask: vi.fn(),
  deleteTask: vi.fn(),
  moveTask: mockMoveTask,
};

vi.mock("@/features/tasks/services/TaskService", () => ({
  useTaskService: () => stableTaskService,
}));

// ------------------------------------------------------
// ⚠️ Maintenant on peut importer BoardDetailsPage
// ------------------------------------------------------
import { describe, it, expect, beforeEach } from "vitest";
import { render, screen, act } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { BoardDetailsPage } from "../BoardDetailsPage";
import { TestProviders } from "@/tests/TestProviders";

const renderPage = () =>
  render(
    <TestProviders>
      <MemoryRouter initialEntries={["/boards/1"]}>
        <Routes>
          <Route path="/boards/:id" element={<BoardDetailsPage />} />
        </Routes>
      </MemoryRouter>
    </TestProviders>
  );

beforeEach(() => mockMoveTask.mockReset());

describe("BoardDetailsPage - Drag & Drop", () => {
  it("déclenche moveTask quand un déplacement est simulé", async () => {
    renderPage();

    // Attendre la fin du loading
    await screen.findByText("Board A");
    await screen.findByText("Task 1");

    await act(async () => {
      window.dispatchEvent(
        new CustomEvent("dnd-move", {
          bubbles: true,
          detail: {
            taskId: "t2",
            toColumnId: "backlog",
            toIndex: 0,
          },
        })
      );
    });

    expect(mockMoveTask).toHaveBeenCalledWith("t2", { columnId: "backlog", order: 0 });
  });
});
