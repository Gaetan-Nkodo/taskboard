import { useApiClient } from "@/api/apiClient";
import { API } from "@/api/endpoints";
import type { Task } from "@/models/Task";

export function useTaskService() {
  const api = useApiClient();

  return {
    getAll: () => api.get<Task[]>(API.tasks),
    getById: (id: string) => api.get<Task>(API.task(id)),
    create: (data: Partial<Task>) => api.post<Task>(API.tasks, data),
    update: (id: string, data: Partial<Task>) => api.put<Task>(API.task(id), data),
    delete: (id: string) => api.delete(API.task(id))
  };
}
