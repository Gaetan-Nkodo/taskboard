import { Menu } from "@headlessui/react";
import type { TaskDto } from "../types/TaskTypes";

interface TaskCardProps {
  task: TaskDto;
  onEdit: (task: TaskDto) => void;
  onDelete: (task: TaskDto) => void;
  onChangeStatus: (task: TaskDto, newStatus: string) => void;
}

export function TaskCard({ task, onEdit, onDelete, onChangeStatus }: TaskCardProps) {
  const statuses = ["Backlog", "Ready", "In Progress", "Review", "Done"];

  return (
    <div className="bg-white dark:bg-gray-800 border rounded-lg p-3 shadow-sm relative group">
      <div className="flex justify-between items-start">
        <h3 className="font-medium text-gray-900 dark:text-gray-100 flex items-center gap-1">
          {task.icon && <span>{task.icon}</span>}
          {task.name}
        </h3>

        <Menu as="div" className="relative inline-block text-left">
          <Menu.Button
            aria-label="menu"
            className="opacity-0 group-hover:opacity-100 transition text-gray-500 hover:text-gray-700"
          >
            ⋮
          </Menu.Button>

          <Menu.Items className="absolute right-0 mt-2 w-44 bg-white dark:bg-gray-700 shadow-lg rounded-md border z-20">
            <Menu.Item>
              {({ active }) => (
                <button
                  onClick={() => onEdit(task)}
                  className={`w-full text-left px-3 py-2 text-sm ${
                    active ? "bg-gray-100 dark:bg-gray-600" : ""
                  }`}
                >
                  ✏️ Modifier
                </button>
              )}
            </Menu.Item>

            <Menu.Item>
              {({ active }) => (
                <button
                  onClick={() => onDelete(task)}
                  className={`w-full text-left px-3 py-2 text-sm text-red-600 ${
                    active ? "bg-gray-100 dark:bg-gray-600" : ""
                  }`}
                >
                  🗑️ Supprimer
                </button>
              )}
            </Menu.Item>

            <div className="border-t my-1" />

            {statuses.map((status) => (
              <Menu.Item key={status}>
                {({ active }) => (
                  <button
                    onClick={() => onChangeStatus(task, status)}
                    className={`w-full text-left px-3 py-2 text-sm ${
                      active ? "bg-gray-100 dark:bg-gray-600" : ""
                    }`}
                  >
                    Déplacer vers {status}
                  </button>
                )}
              </Menu.Item>
            ))}
          </Menu.Items>
        </Menu>
      </div>

      {task.description && (
        <p className="text-sm text-gray-600 dark:text-gray-300 mt-1">
          {task.description}
        </p>
      )}
    </div>
  );
}

export default TaskCard;
