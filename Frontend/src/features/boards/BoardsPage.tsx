import { useEffect, useState } from "react";
import { useBoardService } from "../../core/services/BoardService";
import { useAuthContext } from "../auth/AuthProvider";
import type { Board } from "../../core/models/Board";
import { Link } from "react-router-dom";

export function BoardsPage() {
  const { user, loading } = useAuthContext();
  const boardService = useBoardService();

  const [boards, setBoards] = useState<Board[]>([]);
  const [isLoadingBoards, setIsLoadingBoards] = useState(true);

  useEffect(() => {
    if (loading || !user) return;

    boardService
      .getAll()
      .then(setBoards)
      .finally(() => setIsLoadingBoards(false));
  }, [loading, user, boardService]);

  if (loading) return <div>Chargement...</div>;
  if (!user) return <div>Non autorisé</div>;

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-4">Vos Boards</h1>

      {isLoadingBoards ? (
        <div>Chargement des boards...</div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          {boards.map(board => (
            <Link
              key={board.id}
              to={`/boards/${board.id}`}
              className="p-4 bg-white shadow rounded block hover:bg-gray-50"
            >
              <h2 className="font-semibold">{board.name}</h2>
              <p className="text-sm text-gray-500">{board.description}</p>
            </Link>
          ))}
        </div>
      )}
    </div>
  );
}
