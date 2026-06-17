import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { DeleteBoardModal } from "../DeleteBoardModal";
import type { BoardDto } from "../../types/BoardTypes";

const board: BoardDto = {
  id: "b1",
  name: "Board A",
  description: null,
  columns: [],
  createdAt: "2024-01-01T00:00:00Z",
  updatedAt: "2024-01-01T00:00:00Z",
};

describe("DeleteBoardModal", () => {
  const setup = (props = {}) => {
    const onConfirm = vi.fn().mockResolvedValue(undefined);
    const onCancel = vi.fn();

    render(
      <DeleteBoardModal
        board={board}
        onConfirm={onConfirm}
        onCancel={onCancel}
        {...props}
      />
    );

    return { onConfirm, onCancel };
  };

  // -----------------------------
  // NOT RENDERED WHEN board = null
  // -----------------------------
  it("ne rend rien si board = null", () => {
    const { container } = render(
      <DeleteBoardModal board={null} onConfirm={vi.fn()} onCancel={vi.fn()} />
    );

    expect(container.firstChild).toBeNull();
  });

  // -----------------------------
  // DISPLAY BOARD NAME
  // -----------------------------
  it("affiche le nom du board dans le message", () => {
    setup();

    expect(screen.getByText(/Board A/)).toBeInTheDocument();
  });

  // -----------------------------
  // CANCEL
  // -----------------------------
  it("appelle onCancel quand on clique sur Annuler", () => {
    const { onCancel } = setup();

    fireEvent.click(screen.getByText("Annuler"));

    expect(onCancel).toHaveBeenCalled();
  });

  // -----------------------------
  // CONFIRM
  // -----------------------------
  it("appelle onConfirm avec l'id du board", () => {
    const { onConfirm } = setup();

    fireEvent.click(screen.getByText("Supprimer"));

    expect(onConfirm).toHaveBeenCalledWith("b1");
  });

  // -----------------------------
  // LOADING STATE
  // -----------------------------
  it("désactive les boutons et affiche 'Suppression...' quand loading = true", () => {
    setup({ loading: true });

    const deleteButton = screen.getByText("Suppression...");
    const cancelButton = screen.getByText("Annuler");

    expect(deleteButton).toBeDisabled();
    expect(cancelButton).toBeDisabled();
  });
});
