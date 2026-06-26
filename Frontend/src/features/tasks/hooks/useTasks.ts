import { useState, useEffect, useCallback } from "react";
import { useTaskService } from "../services/TaskService";
import type {
  TaskDto,
  CreateTaskRequest,
  UpdateTaskRequest,
} from "../types/TaskTypes";

export function useTasks(boardId?: string) {
  const service = useTaskService();

  const [tasks, setTasks] = useState<TaskDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadTasks = useCallback(async () => {
    if (!boardId) return;

    setLoading(true);
    setError(null);

    try {
      const data = await service.getTasks(boardId);
      setTasks(data);
    } catch {
      setError("Erreur lors du chargement des tâches");
    } finally {
      setLoading(false);
    }
  }, [boardId, service]);

  useEffect(() => {
    if (!boardId) return;
    void loadTasks();
  }, [boardId, loadTasks]);

  const createTask = useCallback(
    async (payload: CreateTaskRequest) => {
      await service.createTask(boardId!, payload);
      await loadTasks();
    },
    [boardId, service, loadTasks]
  );

  const updateTask = useCallback(
    async (id: string, payload: UpdateTaskRequest) => {
      await service.updateTask(id, payload);
      await loadTasks();
    },
    [service, loadTasks]
  );

  const deleteTask = useCallback(
    async (id: string) => {
      await service.deleteTask(id);
      await loadTasks();
    },
    [service, loadTasks]
  );

  // 🔥 Nouvelle signature compatible tests + BoardDetailsPage
  const moveTask = useCallback(
    async (id: string, payload: { columnId: string; order: number }) => {
      await service.moveTask(id, payload);
      await loadTasks();
    },
    [service, loadTasks]
  );

  return {
    tasks,
    loading,
    error,
    createTask,
    updateTask,
    deleteTask,
    moveTask,
  };
}
