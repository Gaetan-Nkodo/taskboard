import { useHttp } from "@/core/api/httpClient";
import type {
  BoardDto,
  CreateBoardRequest,
  UpdateBoardRequest,
} from "../types/BoardTypes";

let _boardService: ReturnType<typeof createBoardService> | null = null;

export function createBoardService(http: ReturnType<typeof useHttp>) {
  return {
    getBoards: (): Promise<BoardDto[]> =>
      http<BoardDto[]>("/api/v1/boards"),

    getBoard: (id: string): Promise<BoardDto> =>
      http<BoardDto>(`/api/v1/boards/${id}`),

    createBoard: (data: CreateBoardRequest): Promise<BoardDto> =>
      http<BoardDto>("/api/v1/boards", {
        method: "POST",
        body: JSON.stringify(data),
      }),

    updateBoard: (id: string, data: UpdateBoardRequest): Promise<void> =>
      http<void>(`/api/v1/boards/${id}`, {
        method: "PUT",
        body: JSON.stringify(data),
      }),

    deleteBoard: (id: string): Promise<void> =>
      http<void>(`/api/v1/boards/${id}`, {
        method: "DELETE",
      }),
  };
}

export function useBoardService() {
  const http = useHttp();

  if (!_boardService) {
    _boardService = createBoardService(http);
  }

  return _boardService;
}
