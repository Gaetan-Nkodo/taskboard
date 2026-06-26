import { render, screen } from "@testing-library/react";
import { SortableTaskCard } from "../SortableTaskCard";

vi.mock("@dnd-kit/sortable", () => ({
  useSortable: () => ({
    attributes: { "data-sortable": "true" },
    listeners: {},
    setNodeRef: vi.fn(),
    transform: null,
    transition: null,
  }),
}));

describe("SortableTaskCard", () => {
  const task = {
    id: "1",
    name: "Test task",
    columnId: "todo",
    order: 1,
    description: "",
    icon: "",
  };

  it("rend TaskCard", () => {
    render(
      <SortableTaskCard
        task={task}
        onEdit={() => {}}
        onDelete={() => {}}
        onChangeStatus={() => {}}
      />
    );

    expect(screen.getByText("Test task")).toBeInTheDocument();
  });

  it("applique les attributs sortable", () => {
    render(
      <SortableTaskCard
        task={task}
        onEdit={() => {}}
        onDelete={() => {}}
        onChangeStatus={() => {}}
      />
    );

    const sortable = screen.getByTestId("sortable-wrapper");
    expect(sortable).toHaveAttribute("data-sortable", "true");
  });
});
