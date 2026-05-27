import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import type { Board } from "../../core/models/Board";
import { useBoards } from "./useBoards";

export default function BoardPage() {
  const { id } = useParams();
  const { getBoard } = useBoards();
  const [board, setBoard] = useState<Board | null>(null);

  useEffect(() => {
    if (!id) return;
    getBoard(id).then(setBoard);
  }, [id]);

  if (!board) return <p>Chargement…</p>;

  return (
    <div style={{ padding: 20 }}>
      <h1>{board.name}</h1>
      <p>{board.description}</p>
    </div>
  );
}
