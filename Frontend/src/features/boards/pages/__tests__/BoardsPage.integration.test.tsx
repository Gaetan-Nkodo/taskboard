import { describe, it, expect, beforeEach, beforeAll, afterEach, afterAll } from "vitest";
import { render, screen } from "@testing-library/react";
import { setupServer } from "msw/node";
import { http, HttpResponse } from "msw";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { BoardsPage } from "../BoardsPage";
import { TestProviders } from "@/tests/TestProviders";

// Mock data
let boards = [
  { id: "1", name: "Board A", description: "Desc A", columns: [] },
  { id: "2", name: "Board B", description: "Desc B", columns: [] },
];

// MSW server
const server = setupServer(
  http.get(`${import.meta.env.VITE_API_URL}/api/v1/boards`, () => {
    return HttpResponse.json(boards);
  }),

  http.get(`${import.meta.env.VITE_API_URL}/api/v1/boards/:id`, ({ params }) => {
    const board = boards.find((b) => b.id === params.id);
    if (!board) {
      return HttpResponse.json({ message: "Not found" }, { status: 404 });
    }
    return HttpResponse.json(board);
  })
);

beforeAll(() => server.listen());
afterEach(() => server.resetHandlers());
afterAll(() => server.close());

beforeEach(() => {
  localStorage.setItem("accessToken", "abc123");
  localStorage.setItem("refreshToken", "ref123");
  localStorage.setItem("user", JSON.stringify({ id: "1", email: "test@test.com" }));

  boards = [
    { id: "1", name: "Board A", description: "Desc A", columns: [] },
    { id: "2", name: "Board B", description: "Desc B", columns: [] },
  ];
});

const renderPage = () =>
  render(
    <MemoryRouter initialEntries={["/boards"]}>
      <TestProviders>
        <Routes>
          <Route path="/boards" element={<BoardsPage />} />
        </Routes>
      </TestProviders>
    </MemoryRouter>
  );

describe("BoardsPage - Integration", () => {
  it("charge et affiche la liste des boards", async () => {
    renderPage();

    expect(await screen.findByText("Board A")).toBeInTheDocument();
    expect(screen.getByText("Board B")).toBeInTheDocument();
  });

  it("affiche un message si aucun board n'existe", async () => {
    boards = [];

    renderPage();

    expect(await screen.findByText(/aucun board/i)).toBeInTheDocument();
  });
});
