import { apiClient } from "../api/apiClient";
import { API } from "../api/endpoints";
import type { Board } from "../models/Board";

export const BoardService = {
  getAll(): Promise<Board[]> {
    return apiClient.get<Board[]>(API.boards);
  },

  getById(id: string): Promise<Board> {
    return apiClient.get<Board>(API.board(id));
  },

  create(data: { name: string; description?: string }): Promise<Board> {
    return apiClient.post<Board>(API.boards, data);
  },

  update(id: string, data: Partial<Board>): Promise<Board> {
    return apiClient.put<Board>(API.board(id), data);
  },

  delete(id: string): Promise<void> {
    return apiClient.delete<void>(API.board(id));
  }
};
