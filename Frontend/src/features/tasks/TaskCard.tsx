import type { Task } from "../../core/models/Task";

export default function TaskCard({ task }: { task: Task }) {
  return (
    <div
      style={{
        border: "1px solid #ddd",
        padding: "10px",
        borderRadius: "6px",
        marginBottom: "8px",
        background: "#fafafa"
      }}
    >
      <strong>{task.title}</strong>
      <p style={{ margin: 0, fontSize: "0.9em", color: "#666" }}>
        {task.status}
      </p>
    </div>
  );
}
