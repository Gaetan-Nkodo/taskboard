import { useEffect, useState } from "react";
import { useApiClient } from "../../core/api/apiClient";
import { API } from "../../core/api/endpoints";
import type { Board } from "../../core/models/Board";

export function useBoards() {
  const api = useApiClient();
  const [boards, setBoards] = useState<Board[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    api.get<Board[]>(API.boards)
      .then(setBoards)
      .finally(() => setLoading(false));
  }, []);

  return { boards, loading };
}
