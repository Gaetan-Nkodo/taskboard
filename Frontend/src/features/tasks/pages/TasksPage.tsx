import { useMemo } from "react";
import { Link } from "react-router-dom";
import { useBoards } from "@/features/boards/hooks/useBoards";
import type { TaskDto } from "@/features/tasks/types/TaskTypes";

// 🔥 Type enrichi pour cette page uniquement
interface EnrichedTask extends TaskDto {
  boardId: string;
  boardName: string;
  columnName: string;
}

export function TasksPage() {
  const { boards, loading, error } = useBoards();

  const tasks: EnrichedTask[] = useMemo(() => {
    return boards.flatMap((board) =>
      (board.tasks ?? []).map((t) => {
        const col = board.columns.find((c) => c.id === t.columnId);

        return {
          ...t,
          boardId: board.id,
          boardName: board.name,
          columnName: col?.name ?? "Inconnu",
        };
      })
    );
  }, [boards]);

  return (
    <div className="p-6 max-w-5xl mx-auto">
      <h1 className="text-2xl font-bold mb-4">Toutes les tâches</h1>

      <Link
        to="/boards"
        className="text-sm text-blue-600 hover:underline inline-block mb-6"
      >
        ← Retour aux boards
      </Link>

      {loading && (
        <div className="space-y-2">
          <div className="h-6 w-40 bg-gray-200 dark:bg-gray-700 animate-pulse rounded" />
          <div className="h-6 w-64 bg-gray-200 dark:bg-gray-700 animate-pulse rounded" />
          <div className="h-6 w-52 bg-gray-200 dark:bg-gray-700 animate-pulse rounded" />
        </div>
      )}

      {error && <div className="text-red-600 mb-4">{error}</div>}

      {!loading && tasks.length === 0 && (
        <div className="text-gray-500 dark:text-gray-400 italic">
          Aucune tâche trouvée.
        </div>
      )}

      {!loading && tasks.length > 0 && (
        <div className="space-y-3">
          {tasks
            .slice()
            .sort((a, b) => a.name.localeCompare(b.name))
            .map((task) => (
              <div
                key={task.id}
                className="bg-white dark:bg-gray-900 border rounded-lg p-4 shadow-sm"
              >
                <div className="flex justify-between items-center">
                  <div>
                    <div className="font-semibold">{task.name}</div>
                    {task.description && (
                      <div className="text-sm text-gray-500 mt-1">
                        {task.description}
                      </div>
                    )}
                  </div>

                  <Link
                    to={`/boards/${task.boardId}`}
                    className="text-xs text-blue-600 hover:underline"
                  >
                    Voir le board →
                  </Link>
                </div>

                <div className="text-xs text-gray-500 mt-2">
                  Board :{" "}
                  <span className="font-medium">{task.boardName}</span> — Colonne :{" "}
                  <span className="font-medium">{task.columnName}</span>
                </div>
              </div>
            ))}
        </div>
      )}
    </div>
  );
}
