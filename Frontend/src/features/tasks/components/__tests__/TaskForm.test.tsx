import { render, screen, fireEvent } from "@testing-library/react";
import { TaskForm } from "../TaskForm";

describe("TaskForm", () => {
  const columns = [
    { id: "todo", name: "To Do" },
    { id: "inprogress", name: "In Progress" },
  ];

  it("soumet les valeurs", () => {
    const onSubmit = vi.fn();
    const onCancel = vi.fn();

    render(
      <TaskForm
        columns={columns}
        onSubmit={onSubmit}
        onCancel={onCancel}
      />
    );

    fireEvent.change(screen.getByPlaceholderText("Nom de la tâche"), {
      target: { value: "Nouvelle tâche" },
    });

    fireEvent.click(screen.getByText("Enregistrer"));

    expect(onSubmit).toHaveBeenCalledWith({
      name: "Nouvelle tâche",
      description: "",
      columnId: "todo",
    });
  });

  it("annule le formulaire", () => {
    const onSubmit = vi.fn();
    const onCancel = vi.fn();

    render(
      <TaskForm
        columns={columns}
        onSubmit={onSubmit}
        onCancel={onCancel}
      />
    );

    fireEvent.click(screen.getByText("Annuler"));

    expect(onCancel).toHaveBeenCalled();
  });
});
