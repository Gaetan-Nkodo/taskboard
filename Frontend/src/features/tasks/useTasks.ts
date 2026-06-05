import { useEffect, useState } from "react";
import { useApiClient } from "../../core/api/apiClient";
import { API } from "../../core/api/endpoints";
import type { Task } from "../../core/models/Task";

export function useTasks() {
  const api = useApiClient();
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);

  // Charge automatiquement les tâches
  useEffect(() => {
    getTasks().finally(() => setLoading(false));
  }, []);

  async function getTasks() {
    const data = await api.get<Task[]>(API.tasks);
    setTasks(data);
    return data;
  }

  async function createTask(input: { title: string; boardId: string }) {
    const created = await api.post<Task>(API.tasks, input);
    setTasks(prev => [...prev, created]);
    return created;
  }

  return { tasks, loading, getTasks, createTask };
}
