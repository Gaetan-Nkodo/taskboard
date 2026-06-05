import { useEffect, useState } from "react";
import { useApiClient } from "../../core/api/apiClient";
import { API } from "../../core/api/endpoints";
import type { Task } from "../../core/models/Task";

export function useTasks() {
  const api = useApiClient();
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    api.get<Task[]>(API.tasks)
      .then(setTasks)
      .finally(() => setLoading(false));
  }, []);

  return { tasks, loading };
}
