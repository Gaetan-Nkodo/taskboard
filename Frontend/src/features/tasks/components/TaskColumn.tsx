import { SortableContext, verticalListSortingStrategy } from "@dnd-kit/sortable";
import { SortableTaskCard } from "./SortableTaskCard";
import type { ColumnDto } from "../../boards/types/BoardTypes";
import type { TaskDto } from "../types/TaskTypes";

interface Props {
  column: ColumnDto;
  tasks: TaskDto[];
  onCreate: () => void;
  onEdit: (task: TaskDto) => void;
  onDelete: (task: TaskDto) => void;
  onChangeStatus: (task: TaskDto, newColumnId: string) => void;
}

export function TaskColumn({
  column,
  tasks,
  onCreate,
  onEdit,
  onDelete,
  onChangeStatus,
}: Props) {
  const sortedTasks = [...tasks].sort((a, b) => a.order - b.order);

  return (
    <div className="min-w-[260px] bg-gray-50 dark:bg-gray-800 border rounded-lg p-3">
      <h2 className="text-sm font-semibold mb-3 text-gray-900 dark:text-gray-100">
        {column.name}
      </h2>

      <SortableContext
        id={column.id}
        items={sortedTasks.map((t) => t.id)}
        strategy={verticalListSortingStrategy}
      >
        <div
          className="space-y-2"
          data-type="column"
          data-column-id={column.id}
        >
          {sortedTasks.length > 0 ? (
            sortedTasks.map((task) => (
              <SortableTaskCard
                key={task.id}
                task={task}
                onEdit={onEdit}
                onDelete={onDelete}
                onChangeStatus={onChangeStatus}
              />
            ))
          ) : (
            <div className="text-xs text-gray-400 italic">Aucune tâche.</div>
          )}
        </div>
      </SortableContext>

      <button
        className="text-xs text-blue-600 hover:underline mt-3"
        onClick={onCreate}
      >
        + Nouvelle tâche
      </button>
    </div>
  );
}
