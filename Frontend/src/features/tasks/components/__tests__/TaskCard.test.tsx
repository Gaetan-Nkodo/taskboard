import { render, screen, fireEvent } from "@testing-library/react";
import { TaskCard } from "../TaskCard";

describe("TaskCard", () => {
  const task = {
    id: "1",
    name: "Test task",
    columnId: "backlog",
    order: 1,
    description: "",
    icon: "",
  };

  it("affiche le nom", () => {
    render(
      <TaskCard
        task={task}
        onEdit={() => {}}
        onDelete={() => {}}
        onChangeStatus={() => {}}
      />
    );

    expect(screen.getByText("Test task")).toBeInTheDocument();
  });

  it("appelle onEdit", () => {
    const onEdit = vi.fn();

    render(
      <TaskCard
        task={task}
        onEdit={onEdit}
        onDelete={() => {}}
        onChangeStatus={() => {}}
      />
    );

    fireEvent.click(screen.getByLabelText("menu"));
    fireEvent.click(screen.getByText(/Modifier/));

    expect(onEdit).toHaveBeenCalledWith(task);
  });
});
