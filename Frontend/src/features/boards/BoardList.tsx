import { useEffect, useState } from "react";
import type { Board } from "../../core/models/Board";
import { useBoards } from "./useBoards";
import BoardCard from "./BoardCard";

export default function BoardList() {
  const { getBoards } = useBoards();
  const [boards, setBoards] = useState<Board[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getBoards()
      .then(setBoards)
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <p>Chargement des boards…</p>;

  return (
    <div>
      <h2>Boards</h2>
      {boards.map(board => (
        <BoardCard key={board.id} board={board} />
      ))}
    </div>
  );
}
