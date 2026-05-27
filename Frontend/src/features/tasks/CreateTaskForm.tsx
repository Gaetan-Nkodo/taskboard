import { useState } from "react";
import { useTasks } from "./useTasks";

export default function CreateTaskForm({ boardId }: { boardId: string }) {
  const { createTask } = useTasks();
  const [title, setTitle] = useState("");

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!title.trim()) return;

    await createTask({ title, boardId });
    setTitle("");
    alert("Tâche créée !");
  };

  return (
    <form onSubmit={submit} style={{ marginBottom: "20px" }}>
      <input
        value={title}
        onChange={e => setTitle(e.target.value)}
        placeholder="Nouvelle tâche"
        style={{ padding: "8px", marginRight: "8px" }}
      />
      <button type="submit">Créer</button>
    </form>
  );
}
