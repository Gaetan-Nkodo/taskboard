import { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";
import { useBoardService } from "../services/BoardService";
import type { BoardDto } from "../types/BoardTypes";
import { useAuthContext } from "@/features/auth/AuthProvider";

export function BoardDetailsPage() {
  const { id } = useParams<{ id: string }>();
  const service = useBoardService();
  const { accessToken } = useAuthContext();

  const [board, setBoard] = useState<BoardDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!accessToken || !id) return;

    const load = async () => {
      try {
        setLoading(true);
        setError(null);
        const data = await service.getBoard(id);
        setBoard(data);
      } catch (err: unknown) {
        const message = err instanceof Error ? err.message : "Erreur lors du chargement du board";
        setError(message);
      } finally {
        setLoading(false);
      }
    };

    load();
  }, [accessToken, id, service]);

  return (
    <div className="p-6 max-w-7xl mx-auto">
      <Link to="/boards" className="text-sm text-blue-600 hover:underline inline-block mb-4">
        ← Retour aux boards
      </Link>

      {loading ? (
        <div className="h-8 w-48 bg-gray-200 dark:bg-gray-700 animate-pulse rounded mb-2" />
      ) : (
        <h1 className="text-2xl font-bold">{board?.name}</h1>
      )}

      {!loading && board?.description && (
        <p className="text-sm text-gray-600 dark:text-gray-300 mt-1 mb-6">{board.description}</p>
      )}

      {error && <div className="text-red-600 mb-4">{error}</div>}

      {!loading && board && (
        <div className="flex gap-4 overflow-x-auto pb-4">
          {board.columns
            .slice()
            .sort((a, b) => a.order - b.order)
            .map((column) => (
              <div key={column.id} className="min-w-[260px] bg-gray-50 dark:bg-gray-800 border rounded-lg p-3">
                <h2 className="text-sm font-semibold mb-3">{column.name}</h2>

                <div className="space-y-2">
                  {column.tasks
                    .slice()
                    .sort((a, b) => a.order - b.order)
                    .map((task) => (
                      <div key={task.id} className="bg-white dark:bg-gray-900 border rounded-md p-3 text-sm">
                        <div className="font-medium">{task.name}</div>
                        {task.description && (
                          <div className="text-xs text-gray-500 mt-1 line-clamp-2">{task.description}</div>
                        )}
                      </div>
                    ))}

                  {column.tasks.length === 0 && (
                    <div className="text-xs text-gray-400 italic">Aucune tâche.</div>
                  )}
                </div>
              </div>
            ))}
        </div>
      )}
    </div>
  );
}
