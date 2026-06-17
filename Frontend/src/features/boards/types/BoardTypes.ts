export interface CreateBoardRequest {
  name: string;
  description?: string | null;
}

export interface UpdateBoardRequest {
  name: string;
  description?: string | null;
}

export interface TaskDto {
  id: string;
  name: string;
  description?: string | null;
  icon?: string | null;
  order: number;
}

export interface ColumnDto {
  id: string;
  name: string;
  order: number;
  tasks: TaskDto[];
}

export interface BoardDto {
  id: string;
  name: string;
  description?: string | null;
  columns: ColumnDto[];
  createdAt: string;
  updatedAt: string;
}
