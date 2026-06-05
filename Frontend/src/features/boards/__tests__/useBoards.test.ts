import { renderHook } from "@testing-library/react";
import { useBoards } from "../useBoards";

// On mocke useApiClient, pas BoardService
const mockGet = vi.fn();

vi.mock("../../../core/api/apiClient", () => ({
  useApiClient: () => ({
    get: mockGet
  })
}));

describe("useBoards", () => {
  beforeEach(() => {
    mockGet.mockReset();
  });

  it("charge les boards au montage", async () => {
    mockGet.mockResolvedValue([
      { id: "1", name: "Board A", description: "Desc A" }
    ]);

    const { result } = renderHook(() => useBoards());

    // Attendre la fin du chargement
    await vi.waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    // Vérifier les données
    expect(result.current.boards).toEqual([
      { id: "1", name: "Board A", description: "Desc A" }
    ]);

    // Vérifier que l’API a été appelée une seule fois
    expect(mockGet).toHaveBeenCalledTimes(1);
    expect(mockGet).toHaveBeenCalledWith("/api/v1/boards");
  });
});
