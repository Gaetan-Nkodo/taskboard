import { useState, useCallback, useEffect } from "react";
import { useBoardService } from "../services/BoardService";
import type { BoardDto, CreateBoardRequest, UpdateBoardRequest } from "../types/BoardTypes";
import { useAuthContext } from "@/features/auth/AuthProvider";

export function useBoards() {
  const service = useBoardService();
  const { accessToken } = useAuthContext();

  const [boards, setBoards] = useState<BoardDto[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const loadBoards = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);

      const data = await service.getBoards();
      setBoards(data);

    } catch (err) {
      const message =
        err instanceof Error ? err.message : "Erreur lors du chargement des boards";
      setError(message);

    } finally {
      setLoading(false);
    }
  }, [service]);

  useEffect(() => {
    if (!accessToken) return;
    loadBoards();
  }, [accessToken, loadBoards]);

  const createBoard = useCallback(
    async (payload: CreateBoardRequest) => {
      const result = await service.createBoard(payload);
      await loadBoards();
      return result.id;
    },
    [service, loadBoards]
  );

  const updateBoard = useCallback(
    async (id: string, payload: UpdateBoardRequest) => {
      await service.updateBoard(id, payload);
      await loadBoards();
    },
    [service, loadBoards]
  );

  const deleteBoard = useCallback(
    async (id: string) => {
      await service.deleteBoard(id);
      await loadBoards();
    },
    [service, loadBoards]
  );

  return {
    boards,
    loading,
    error,
    reload: loadBoards,
    createBoard,
    updateBoard,
    deleteBoard,
  };
}
