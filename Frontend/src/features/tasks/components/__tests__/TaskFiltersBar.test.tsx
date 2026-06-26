import { render, screen, fireEvent } from "@testing-library/react";
import { TaskFiltersBar } from "../TaskFiltersBar";

describe("TaskFiltersBar (advanced)", () => {
  const assignees = [
    { id: "u1", name: "Alice" },
    { id: "u2", name: "Bob" },
  ];

  const props = {
    search: "",
    status: "all",
    assignee: "all",
    sort: "order",
    assignees,
    onSearchChange: vi.fn(),
    onStatusChange: vi.fn(),
    onAssigneeChange: vi.fn(),
    onSortChange: vi.fn(),
    onReset: vi.fn(),
  };

  it("modifie la recherche", () => {
    render(<TaskFiltersBar {...props} />);

    fireEvent.change(screen.getByPlaceholderText("Rechercher une tâche..."), {
      target: { value: "test" },
    });

    expect(props.onSearchChange).toHaveBeenCalledWith("test");
  });

  it("modifie le statut", () => {
    render(<TaskFiltersBar {...props} />);

    fireEvent.change(screen.getByDisplayValue("Tous les statuts"), {
      target: { value: "Done" },
    });

    expect(props.onStatusChange).toHaveBeenCalledWith("Done");
  });

  it("modifie l'assignee", () => {
    render(<TaskFiltersBar {...props} />);

    fireEvent.change(screen.getByDisplayValue("Tous les assignés"), {
      target: { value: "u1" },
    });

    expect(props.onAssigneeChange).toHaveBeenCalledWith("u1");
  });

  it("modifie le tri", () => {
    render(<TaskFiltersBar {...props} />);

    fireEvent.change(screen.getByDisplayValue("Ordre"), {
      target: { value: "name" },
    });

    expect(props.onSortChange).toHaveBeenCalledWith("name");
  });

  it("réinitialise les filtres", () => {
    render(<TaskFiltersBar {...props} />);

    fireEvent.click(screen.getByText("Réinitialiser"));

    expect(props.onReset).toHaveBeenCalled();
  });
});
