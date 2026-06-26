import { useHttp } from "@/core/api/httpClient";
import type {
  TaskDto,
  CreateTaskRequest,
  UpdateTaskRequest,
} from "../types/TaskTypes";

let _taskService: ReturnType<typeof createTaskService> | null = null;

export function createTaskService(http: ReturnType<typeof useHttp>) {
  return {
    getTasks: (boardId: string): Promise<TaskDto[]> =>
      http<TaskDto[]>(`/api/v1/boards/${boardId}/tasks`),

    createTask: (boardId: string, data: CreateTaskRequest): Promise<TaskDto> =>
      http<TaskDto>(`/api/v1/boards/${boardId}/tasks`, {
        method: "POST",
        body: JSON.stringify(data),
      }),

    updateTask: (id: string, data: UpdateTaskRequest): Promise<void> =>
      http<void>(`/api/v1/tasks/${id}`, {
        method: "PUT",
        body: JSON.stringify(data),
      }),

    deleteTask: (id: string): Promise<void> =>
      http<void>(`/api/v1/tasks/${id}`, {
        method: "DELETE",
      }),

    moveTask: (id: string, data: { columnId: string; order: number }): Promise<void> =>
      http<void>(`/api/v1/tasks/${id}/move`, {
        method: "PATCH",
        body: JSON.stringify(data),
      }),
  };
}

export function useTaskService() {
  const http = useHttp();

  if (!_taskService) {
    _taskService = createTaskService(http);
  }

  return _taskService;
}
