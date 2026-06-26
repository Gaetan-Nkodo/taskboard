export interface CreateTaskRequest {
  columnId: string;
  name: string;
  description: string | null;
  icon?: string | null;
}

export interface UpdateTaskRequest {
  columnId?: string;
  name?: string;
  description?: string | null;
  icon?: string | null;
  order?: number;
}

export interface TaskDto {
  id: string;
  name: string;
  columnId: string;
  order: number;
  boardId?: string;
  description?: string | null;
  icon?: string | null;
  assigneeId?: string | null;
  dueDate?: string | null;
  priority?: string | null;
  createdAt?: string;
}
