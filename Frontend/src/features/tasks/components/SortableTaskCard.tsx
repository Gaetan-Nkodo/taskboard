import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import type { TaskDto } from "../types/TaskTypes";
import type { ColumnDto } from "@/features/boards/types/BoardTypes";

interface Props {
  task: TaskDto;
  onEdit: (task: TaskDto) => void;
  onDelete: (task: TaskDto) => void;
  onChangeStatus: (task: TaskDto, newColumnId: string) => void;
  columns?: ColumnDto[];
}

export function SortableTaskCard({
  task,
  onEdit,
  onDelete,
  onChangeStatus,
  columns = [],
}: Props) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: task.id });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.5 : 1,
  };

  return (
    <div
      data-testid="sortable-wrapper"
      ref={setNodeRef}
      style={style}
      {...attributes}
      {...listeners}
      className="bg-white dark:bg-gray-700 border rounded p-3 shadow-sm cursor-grab"
    >
      <div className="flex justify-between items-start">
        <div>
          <div className="font-medium text-gray-900 dark:text-gray-100">
            {task.icon && <span className="mr-1">{task.icon}</span>}
            {task.name}
          </div>

          {task.description && (
            <div className="text-xs text-gray-500 dark:text-gray-300 mt-1">
              {task.description}
            </div>
          )}
        </div>

        <div className="flex gap-2">
          <button
            className="text-xs text-blue-600 hover:underline"
            onClick={() => onEdit(task)}
          >
            Éditer
          </button>

          <button
            className="text-xs text-red-600 hover:underline"
            onClick={() => onDelete(task)}
          >
            Supprimer
          </button>
        </div>
      </div>

      {columns.length > 0 && (
        <div className="mt-3">
          <select
            className="text-xs border rounded px-2 py-1 dark:bg-gray-600 dark:text-gray-100"
            value={task.columnId}
            onChange={(e) => onChangeStatus(task, e.target.value)}
          >
            {columns.map((col) => (
              <option key={col.id} value={col.id}>
                {col.name}
              </option>
            ))}
          </select>
        </div>
      )}
    </div>
  );
}
