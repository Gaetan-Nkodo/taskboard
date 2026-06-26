import type { TaskDto } from "@/features/tasks/types/TaskTypes";
export interface CreateBoardRequest {
  name: string;
  description?: string | null;
}

export interface UpdateBoardRequest {
  name: string;
  description?: string | null;
}

export interface ColumnDto {
  id: string;
  name: string;
  order: number;
}

export interface BoardDto {
  id: string;
  name: string;
  description?: string | null;
  columns: ColumnDto[];
  tasks?: TaskDto[];
  createdAt: string;
  updatedAt: string;
}
