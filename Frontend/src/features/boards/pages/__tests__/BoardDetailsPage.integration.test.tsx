import { describe, it, expect, beforeEach } from "vitest";
import { render, screen } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { BoardDetailsPage } from "../BoardDetailsPage";
import { TestProviders } from "@/tests/TestProviders";
import { server } from "@/tests/msw-server";
import { http, HttpResponse } from "msw";

const renderPage = (id = "1") =>
  render(
    <MemoryRouter initialEntries={[`/boards/${id}`]}>
      <TestProviders>
        <Routes>
          <Route path="/boards/:id" element={<BoardDetailsPage />} />
        </Routes>
      </TestProviders>
    </MemoryRouter>
  );

beforeEach(() => {
  localStorage.setItem("accessToken", "abc123");
  localStorage.setItem("refreshToken", "ref123");
  localStorage.setItem("user", JSON.stringify({ id: "1", email: "test@test.com" }));
});

describe("BoardDetailsPage - Integration", () => {
  it("charge et affiche le board", async () => {
    renderPage();

    expect(await screen.findByText("Board A")).toBeInTheDocument();
    expect(screen.getByText("Desc A")).toBeInTheDocument();
  });

  it("affiche les colonnes et tâches", async () => {
    renderPage();

    await screen.findByText("Board A");

    // Nouvelles colonnes Trello-like
    expect(screen.getByText("Backlog")).toBeInTheDocument();
    expect(screen.getByText("Ready")).toBeInTheDocument();
    expect(screen.getByText("In Progress")).toBeInTheDocument();
    expect(screen.getByText("Review")).toBeInTheDocument();
    expect(screen.getByText("Done")).toBeInTheDocument();

    // Tâches mockées par MSW
    expect(screen.getByText("Task 1")).toBeInTheDocument();
  });

  it("affiche 'Aucune tâche.'", async () => {
    server.use(
      http.get("/api/v1/boards/:id/tasks", () =>
        HttpResponse.json([])
      )
    );

    renderPage();

    await screen.findByText("Board A");

    const emptyMessages = screen.getAllByText("Aucune tâche.");
    expect(emptyMessages.length).toBeGreaterThan(0);
  });

  it("affiche une erreur si le board n'existe pas", async () => {
    server.use(
      http.get("/api/v1/boards/:id", () =>
        HttpResponse.json({ message: "Not found" }, { status: 404 })
      )
    );

    renderPage("999");

    expect(await screen.findByText("Erreur lors du chargement du board"))
      .toBeInTheDocument();
  });
});
