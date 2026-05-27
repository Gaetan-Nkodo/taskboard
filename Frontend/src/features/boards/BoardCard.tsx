import type { Board } from "../../core/models/Board";
import { Link } from "react-router-dom";

export default function BoardCard({ board }: { board: Board }) {
  return (
    <Link
      to={`/boards/${board.id}`}
      style={{
        display: "block",
        padding: "12px",
        border: "1px solid #ddd",
        borderRadius: "6px",
        marginBottom: "10px",
        textDecoration: "none",
        color: "inherit"
      }}
    >
      <strong>{board.name}</strong>
      {board.description && (
        <p style={{ margin: 0, fontSize: "0.9em", color: "#666" }}>
          {board.description}
        </p>
      )}
    </Link>
  );
}
