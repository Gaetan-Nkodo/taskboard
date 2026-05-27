import { useEffect, useState } from "react";
import type { Task } from "../../core/models/Task";
import { useTasks } from "./useTasks";
import TaskCard from "./TaskCard";

export default function TaskList() {
  const { getTasks } = useTasks();
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getTasks()
      .then(setTasks)
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <p>Chargement des tâches…</p>;

  return (
    <div>
      <h2>Tâches</h2>
      {tasks.map(task => (
        <TaskCard key={task.id} task={task} />
      ))}
    </div>
  );
}
