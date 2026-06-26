import { render, screen, fireEvent, act } from "@testing-library/react";
import { TaskFormModal } from "../TaskFormModal";

describe("TaskFormModal", () => {
  const columns = [
    { id: "c1", name: "Backlog", order: 1, tasks: [] },
    { id: "c2", name: "Done", order: 2, tasks: [] },
  ];

  it("soumet en mode création", async () => {
    const onSubmit = vi.fn().mockResolvedValue(undefined);
    const onClose = vi.fn();

    render(
      <TaskFormModal
        open={true}
        title="Créer"
        columns={columns}
        onSubmit={onSubmit}
        onClose={onClose}
      />
    );

    fireEvent.change(screen.getByLabelText("Nom"), {
      target: { value: "New Task" },
    });

    await act(async () => {
      fireEvent.click(screen.getByText("Enregistrer"));
    });

    expect(onSubmit).toHaveBeenCalledWith({
      name: "New Task",
      description: null,
      icon: null,
      columnId: "c1",
    });
  });

  it("pré-remplit en mode édition", () => {
    const task = {
      id: "t1",
      name: "Old",
      description: "Desc",
      icon: "🔥",
      columnId: "c2",
      order: 1,
    };

    render(
      <TaskFormModal
        open={true}
        title="Modifier"
        initialValues={task}
        columns={columns}
        onSubmit={() => Promise.resolve()}
        onClose={() => {}}
      />
    );

    expect(screen.getByDisplayValue("Old")).toBeInTheDocument();
    expect(screen.getByDisplayValue("Desc")).toBeInTheDocument();
    expect(screen.getByDisplayValue("🔥")).toBeInTheDocument();
    expect(screen.getByDisplayValue("Done")).toBeInTheDocument();
  });
});
