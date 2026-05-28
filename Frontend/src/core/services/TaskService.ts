import { apiClient } from "../api/apiClient";
import { API } from "../api/endpoints";
import type { Task } from "../models/Task";

export const TaskService = {
  getAll(): Promise<Task[]> {
    return apiClient.get<Task[]>(API.tasks);
  },

  getById(id: string): Promise<Task> {
    return apiClient.get<Task>(API.task(id));
  },

  create(data: { title: string; boardId: string; columnId: string }): Promise<Task> {
    return apiClient.post<Task>(API.tasks, data);
  },

  update(id: string, data: Partial<Task>): Promise<Task> {
    return apiClient.put<Task>(API.task(id), data);
  },

  delete(id: string): Promise<void> {
    return apiClient.delete<void>(API.task(id));
  }
};
