import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, waitFor } from "@testing-library/react";
import { useTasks } from "../useTasks";

// --- MOCK useApiClient ---
const mockGet = vi.fn();

vi.mock("@/core/api/apiClient", () => ({
  useApiClient: () => ({
    get: mockGet
  })
}));

describe("useTasks", () => {
  beforeEach(() => {
    mockGet.mockReset();
  });

  it("charge les tâches au montage", async () => {
    mockGet.mockResolvedValue([
      { id: "1", title: "Test", status: "Todo", boardId: "b1" }
    ]);

    const { result } = renderHook(() => useTasks());

    // loading = true au début
    expect(result.current.loading).toBe(true);

    await waitFor(() => {
      expect(mockGet).toHaveBeenCalledTimes(1);
      expect(result.current.tasks).toHaveLength(1);
      expect(result.current.loading).toBe(false);
    });
  });
});
