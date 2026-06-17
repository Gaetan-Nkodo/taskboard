import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, act } from "@testing-library/react";
import { useBoards } from "../useBoards";
import { AuthProvider } from "@/features/auth/AuthProvider";

// Mock du BoardService
const mockService = {
  getBoards: vi.fn(),
  createBoard: vi.fn(),
  updateBoard: vi.fn(),
  deleteBoard: vi.fn(),
};

vi.mock("../../services/BoardService", () => ({
  useBoardService: () => mockService,
}));

describe("useBoards", () => {
  beforeEach(() => {
    vi.clearAllMocks();

    // 🔥 IMPORTANT : initialiser l’auth sinon useBoards ne fetch pas
    localStorage.setItem("accessToken", "abc123");
    localStorage.setItem("refreshToken", "ref123");
    localStorage.setItem("user", JSON.stringify({ id: "1", email: "test@test.com" }));
  });

  // Wrapper pour injecter AuthProvider
  const wrapper = ({ children }: { children: React.ReactNode }) => (
    <AuthProvider>{children}</AuthProvider>
  );

  // -----------------------------
  // LOAD BOARDS
  // -----------------------------
  it("charge les boards au montage", async () => {
    mockService.getBoards.mockResolvedValue([
      { id: "1", name: "Board A", description: null, columns: [] },
    ]);

    const { result } = renderHook(() => useBoards(), { wrapper });

    expect(result.current.loading).toBe(true);

    await act(async () => {});

    expect(result.current.loading).toBe(false);
    expect(result.current.boards).toEqual([
      { id: "1", name: "Board A", description: null, columns: [] },
    ]);
    expect(mockService.getBoards).toHaveBeenCalledTimes(1);
  });

  // -----------------------------
  // CREATE
  // -----------------------------
  it("createBoard crée un board et recharge la liste", async () => {
    mockService.getBoards.mockResolvedValue([]);
    mockService.createBoard.mockResolvedValue({ id: "new-id" });

    const { result } = renderHook(() => useBoards(), { wrapper });

    await act(async () => {});

    await act(async () => {
      await result.current.createBoard({ name: "Test", description: null });
    });

    expect(mockService.createBoard).toHaveBeenCalledWith({
      name: "Test",
      description: null,
    });

    expect(mockService.getBoards).toHaveBeenCalledTimes(2);
  });

  // -----------------------------
  // UPDATE
  // -----------------------------
  it("updateBoard met à jour un board et recharge la liste", async () => {
    mockService.getBoards.mockResolvedValue([]);
    mockService.updateBoard.mockResolvedValue(undefined);

    const { result } = renderHook(() => useBoards(), { wrapper });

    await act(async () => {});

    await act(async () => {
      await result.current.updateBoard("1", {
        name: "Updated",
        description: null,
      });
    });

    expect(mockService.updateBoard).toHaveBeenCalledWith("1", {
      name: "Updated",
      description: null,
    });

    expect(mockService.getBoards).toHaveBeenCalledTimes(2);
  });

  // -----------------------------
  // DELETE
  // -----------------------------
  it("deleteBoard supprime un board et recharge la liste", async () => {
    mockService.getBoards.mockResolvedValue([]);
    mockService.deleteBoard.mockResolvedValue(undefined);

    const { result } = renderHook(() => useBoards(), { wrapper });

    await act(async () => {});

    await act(async () => {
      await result.current.deleteBoard("1");
    });

    expect(mockService.deleteBoard).toHaveBeenCalledWith("1");
    expect(mockService.getBoards).toHaveBeenCalledTimes(2);
  });

  // -----------------------------
  // ERROR HANDLING
  // -----------------------------
  it("gère les erreurs lors du chargement", async () => {
    mockService.getBoards.mockRejectedValue(new Error("Boom"));

    const { result } = renderHook(() => useBoards(), { wrapper });

    await act(async () => {});

    expect(result.current.error).toBe("Boom");
    expect(result.current.loading).toBe(false);
    expect(result.current.boards).toEqual([]);
  });
});
