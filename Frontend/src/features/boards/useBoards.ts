import { useEffect, useState } from "react";
import { useApiClient } from "@/core/api/apiClient";
import { API } from "@/core/api/endpoints";
import type { Board } from "@/core/models/Board";

export function useBoards() {
  const api = useApiClient();
  const [boards, setBoards] = useState<Board[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getBoards().finally(() => setLoading(false));
  }, []);

  async function getBoards() {
    const data = await api.get<Board[]>(API.boards);
    setBoards(data);
    return data;
  }

  async function createBoard(input: { name: string; description: string }) {
    const created = await api.post<Board>(API.boards, input);
    setBoards(prev => [...prev, created]);
    return created;
  }

  return { boards, loading, getBoards, createBoard };
}
