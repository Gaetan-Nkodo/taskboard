import { useApiClient } from "../api/apiClient";
import { API } from "../api/endpoints";
import type { Board } from "../models/Board";

export function useBoardService() {
  const api = useApiClient();

  return {
    getAll: () => api.get<Board[]>(API.boards),
    getById: (id: string) => api.get<Board>(API.board(id)),
    create: (data: Partial<Board>) => api.post<Board>(API.boards, data),
    update: (id: string, data: Partial<Board>) => api.put<Board>(API.board(id), data),
    delete: (id: string) => api.delete(API.board(id))
  };
}
