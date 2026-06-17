import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { BoardForm } from "../BoardForm";

describe("BoardForm", () => {
  const setup = (props = {}) => {
    const onSubmit = vi.fn().mockResolvedValue(undefined);
    const onCancel = vi.fn();

    render(
      <BoardForm
        onSubmit={onSubmit}
        onCancel={onCancel}
        {...props}
      />
    );

    return { onSubmit, onCancel };
  };

  // -----------------------------
  // VALIDATION
  // -----------------------------
  it("affiche une erreur si le nom est vide", async () => {
    const { onSubmit } = setup();

    fireEvent.click(screen.getByText("Enregistrer"));

    expect(await screen.findByText("Le nom du board est requis.")).toBeInTheDocument();
    expect(onSubmit).not.toHaveBeenCalled();
  });

  // -----------------------------
  // SUBMIT
  // -----------------------------
  it("appelle onSubmit avec les bonnes valeurs", async () => {
    const { onSubmit } = setup();

    fireEvent.change(screen.getByLabelText("Nom du board"), {
      target: { value: "Nouveau Board" },
    });

    fireEvent.change(screen.getByLabelText("Description"), {
      target: { value: "Une description" },
    });

    fireEvent.click(screen.getByText("Enregistrer"));

    expect(onSubmit).toHaveBeenCalledWith({
      name: "Nouveau Board",
      description: "Une description",
    });
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
  // LOADING STATE
  // -----------------------------
  it("affiche 'Enregistrement...' quand loading = true", () => {
    setup({ loading: true });

    expect(screen.getByText("Enregistrement...")).toBeInTheDocument();
  });

  // -----------------------------
  // INITIAL VALUES
  // -----------------------------
  it("pré-remplit les champs avec initialValues", () => {
    setup({
      initialValues: {
        name: "Board existant",
        description: "Desc existante",
      },
    });

    expect(screen.getByDisplayValue("Board existant")).toBeInTheDocument();
    expect(screen.getByDisplayValue("Desc existante")).toBeInTheDocument();
  });
});
