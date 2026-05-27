import { http } from "../../core/api/httpClient";
import { API } from "../../core/api/endpoints";
import type { Task } from "../../core/models/Task";

export const useTasks = () => {
  const getTasks = () => http<Task[]>(API.tasks);

  const createTask = (data: { title: string; boardId: string }) =>
    http<Task>(API.tasks, {
      method: "POST",
      body: JSON.stringify(data)
    });

  const updateTask = (id: string, data: Partial<Task>) =>
    http<Task>(`${API.tasks}/${id}`, {
      method: "PUT",
      body: JSON.stringify(data)
    });

  return { getTasks, createTask, updateTask };
};
