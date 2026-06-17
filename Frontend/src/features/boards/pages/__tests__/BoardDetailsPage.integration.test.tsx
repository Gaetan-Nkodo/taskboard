import { describe, it, expect, beforeEach, beforeAll, afterEach, afterAll } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import { setupServer } from "msw/node";
import { http, HttpResponse } from "msw";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { BoardDetailsPage } from "../BoardDetailsPage";
import { TestProviders } from "@/tests/TestProviders";

let board = {
  id: "1",
  name: "Board A",
  description: "Desc A",
  columns: [
    {
      id: "c1",
      name: "Todo",
      order: 1,
      tasks: [
        { id: "t1", name: "Task 1", description: "Task 1 desc", order: 1 },
      ],
    },
    {
      id: "c2",
      name: "Done",
      order: 2,
      tasks: [],
    },
  ],
};

const server = setupServer(
  http.get(`${import.meta.env.VITE_API_URL}/api/v1/boards/:id`, ({ params }) => {
    if (params.id !== "1") {
      return HttpResponse.json({ message: "Not found" }, { status: 404 });
    }
    return HttpResponse.json(board);
  })
);

beforeAll(() => server.listen());
afterEach(() => server.resetHandlers());
afterAll(() => server.close());

beforeEach(() => {
  // 🔥 ESSENTIEL : initialiser l’auth sinon useHttp rejette
  localStorage.setItem("accessToken", "abc123");
  localStorage.setItem("refreshToken", "ref123");
  localStorage.setItem("user", JSON.stringify({ id: "1", email: "test@test.com" }));

  board = {
    id: "1",
    name: "Board A",
    description: "Desc A",
    columns: [
      {
        id: "c1",
        name: "Todo",
        order: 1,
        tasks: [
          { id: "t1", name: "Task 1", description: "Task 1 desc", order: 1 },
        ],
      },
      {
        id: "c2",
        name: "Done",
        order: 2,
        tasks: [],
      },
    ],
  };
});

const renderPage = () =>
  render(
    <MemoryRouter initialEntries={["/boards/1"]}>
      {/* 🔥 TestProviders doit inclure AuthProvider */}
      <TestProviders>
        <Routes>
          <Route path="/boards/:id" element={<BoardDetailsPage />} />
        </Routes>
      </TestProviders>
    </MemoryRouter>
  );

describe("BoardDetailsPage - Integration", () => {
  it("charge et affiche le board", async () => {
    renderPage();

    expect(await screen.findByText("Board A")).toBeInTheDocument();
    expect(screen.getByText("Desc A")).toBeInTheDocument();
  });

  it("affiche les colonnes et tâches", async () => {
    renderPage();

    await screen.findByText("Board A");

    expect(screen.getByText("Todo")).toBeInTheDocument();
    expect(screen.getByText("Task 1")).toBeInTheDocument();
  });

  it("affiche 'Aucune tâche.'", async () => {
    renderPage();

    await screen.findByText("Board A");

    expect(screen.getByText("Aucune tâche.")).toBeInTheDocument();
  });

it("affiche une erreur si le board n'existe pas", async () => {
  render(
    <MemoryRouter initialEntries={["/boards/999"]}>
      <TestProviders>
        <Routes>
          <Route path="/boards/:id" element={<BoardDetailsPage />} />
        </Routes>
      </TestProviders>
    </MemoryRouter>
  );

  await waitFor(() => {
    expect(
      screen.getByText(/Erreur lors du chargement du board/i)
    ).toBeInTheDocument();
  });
});});
