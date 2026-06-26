import { render, screen, fireEvent } from "@testing-library/react";
import { TaskColumn } from "../TaskColumn";
import type { ColumnDto } from "../../../boards/types/BoardTypes";
import type { TaskDto } from "../../types/TaskTypes";

describe("TaskColumn", () => {
  const column: ColumnDto = {
    id: "backlog",
    name: "Backlog",
    order: 0,
  };

  const tasks: TaskDto[] = [
    { id: "2", name: "Task B", columnId: "backlog", order: 2 },
    { id: "1", name: "Task A", columnId: "backlog", order: 1 },
  ];

  it("affiche les tâches triées par order", () => {
    render(
      <TaskColumn
        column={column}
        tasks={tasks}
        onCreate={() => {}}
        onEdit={() => {}}
        onDelete={() => {}}
        onChangeStatus={() => {}}
      />
    );

    const items = screen.getAllByText(/Task/).map((el) => el.textContent);

    expect(items).toEqual(["Task A", "Task B"]);
  });

  it("affiche 'Aucune tâche' si vide", () => {
    render(
      <TaskColumn
        column={column}
        tasks={[]}
        onCreate={() => {}}
        onEdit={() => {}}
        onDelete={() => {}}
        onChangeStatus={() => {}}
      />
    );

    expect(screen.getByText("Aucune tâche.")).toBeInTheDocument();
  });

  it("appelle onCreate", () => {
    const onCreate = vi.fn();

    render(
      <TaskColumn
        column={column}
        tasks={tasks}
        onCreate={onCreate}
        onEdit={() => {}}
        onDelete={() => {}}
        onChangeStatus={() => {}}
      />
    );

    fireEvent.click(screen.getByText("+ Nouvelle tâche"));
    expect(onCreate).toHaveBeenCalled();
  });
});
