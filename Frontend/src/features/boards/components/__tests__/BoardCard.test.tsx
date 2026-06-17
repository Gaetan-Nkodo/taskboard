import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { BoardCard } from "../BoardCard";
import type { BoardDto } from "../../types/BoardTypes";

const board: BoardDto = {
  id: "1",
  name: "Test",
  description: "",
  createdAt: "",
  updatedAt: "",
  columns: [],
};

describe("BoardCard", () => {
  let onOpen: any;
  let onEdit: any;
  let onDelete: any;

  beforeEach(() => {
    onOpen = vi.fn();
    onEdit = vi.fn();
    onDelete = vi.fn();
  });

  const renderCard = () =>
    render(
      <BoardCard
        board={board}
        onOpen={onOpen}
        onEdit={onEdit}
        onDelete={onDelete}
      />
    );

  // -----------------------------
  // OPEN BOARD
  // -----------------------------
  it("appelle onOpen quand on clique sur la carte", () => {
    renderCard();

    fireEvent.click(screen.getByText("Board A"));

    expect(onOpen).toHaveBeenCalledWith(board);
  });

  // -----------------------------
  // MENU OPEN
  // -----------------------------
  it("ouvre le menu quand on clique sur le bouton ellipsis", () => {
    renderCard();

    const menuButton = screen.getByRole("button");
    fireEvent.click(menuButton);

    expect(screen.getByText("Modifier")).toBeInTheDocument();
    expect(screen.getByText("Supprimer")).toBeInTheDocument();
  });

  // -----------------------------
  // EDIT
  // -----------------------------
  it("appelle onEdit quand on clique sur Modifier", () => {
    renderCard();

    const menuButton = screen.getByRole("button");
    fireEvent.click(menuButton);

    fireEvent.click(screen.getByText("Modifier"));

    expect(onEdit).toHaveBeenCalledWith(board);
  });

  // -----------------------------
  // DELETE
  // -----------------------------
  it("appelle onDelete quand on clique sur Supprimer", () => {
    renderCard();

    const menuButton = screen.getByRole("button");
    fireEvent.click(menuButton);

    fireEvent.click(screen.getByText("Supprimer"));

    expect(onDelete).toHaveBeenCalledWith(board);
  });

  // -----------------------------
  // STOP PROPAGATION
  // -----------------------------
  it("ne déclenche pas onOpen quand on clique sur le menu", () => {
    renderCard();

    const menuButton = screen.getByRole("button");
    fireEvent.click(menuButton);

    expect(onOpen).not.toHaveBeenCalled();
  });
});
