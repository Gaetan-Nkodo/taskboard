import { describe, it, expect, beforeEach } from "vitest";
import { render, screen } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { BoardsPage } from "../BoardsPage";
import { TestProviders } from "@/tests/TestProviders";
import { server } from "@/tests/msw-server";
import { http, HttpResponse } from "msw";

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

beforeEach(() => {
  localStorage.setItem("accessToken", "abc123");
  localStorage.setItem("refreshToken", "ref123");
  localStorage.setItem("user", JSON.stringify({ id: "1", email: "test@test.com" }));
});

const urls = (path: string) => [
  path,
  `http://localhost${path}`,
  `http://localhost:3000${path}`,
];

describe("BoardsPage - Integration", () => {
  it("charge et affiche la liste des boards", async () => {
    renderPage();

    expect(await screen.findByText("Board A")).toBeInTheDocument();
    expect(screen.getByText("Board B")).toBeInTheDocument();
  });

  it("affiche un message si aucun board n'existe", async () => {
    server.use(
      ...urls("/api/v1/boards").map(url =>
        http.get(url, () => HttpResponse.json([]))
      )
    );

    renderPage();

    expect(await screen.findByText(/Aucun board/i)).toBeInTheDocument();
  });
});
