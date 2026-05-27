import { http } from "../../core/api/httpClient";
import { API } from "../../core/api/endpoints";
import type { Board } from "../../core/models/Board";

export const useBoards = () => {
  const getBoards = () => http<Board[]>(API.boards);

  const getBoard = (id: string) =>
    http<Board>(`${API.boards}/${id}`);

  const createBoard = (data: { name: string; description?: string }) =>
    http<Board>(API.boards, {
      method: "POST",
      body: JSON.stringify(data)
    });

  return { getBoards, getBoard, createBoard };
};
