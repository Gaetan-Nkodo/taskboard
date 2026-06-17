import { useMemo } from "react";
import { useHttp } from "@/core/api/httpClient";
import type {
  CreateBoardRequest,
  UpdateBoardRequest,
  BoardDto
} from "../types/BoardTypes";

export function createBoardService(http: ReturnType<typeof useHttp>) {
  return {
    getBoards(): Promise<BoardDto[]> {
      return http("/api/v1/boards");
    },

    getBoard(id: string): Promise<BoardDto> {
      return http(`/api/v1/boards/${id}`);
    },

    createBoard(data: CreateBoardRequest): Promise<{ id: string }> {
      return http("/api/v1/boards", {
        method: "POST",
        body: JSON.stringify(data),
      });
    },

    updateBoard(id: string, data: UpdateBoardRequest): Promise<void> {
      return http(`/api/v1/boards/${id}`, {
        method: "PUT",
        body: JSON.stringify(data),
      });
    },

    deleteBoard(id: string): Promise<void> {
      return http(`/api/v1/boards/${id}`, {
        method: "DELETE",
      });
    },
  };
}

export function useBoardService() {
  const http = useHttp();
  return useMemo(() => createBoardService(http), [http]);
}
