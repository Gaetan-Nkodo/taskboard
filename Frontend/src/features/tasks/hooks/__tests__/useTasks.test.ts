import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, waitFor, act } from "@testing-library/react";
import { useTasks } from "../useTasks";

vi.mock("@/features/auth/AuthProvider", () => ({
  useAuthContext: () => ({ accessToken: "TOKEN" }),
}));

const mockGetTasks = vi.fn();
const mockCreateTask = vi.fn();
const mockUpdateTask = vi.fn();
const mockDeleteTask = vi.fn();
const mockMoveTask = vi.fn();

vi.mock("@/features/tasks/services/TaskService", () => ({
  useTaskService: () => ({
    getTasks: mockGetTasks,
    createTask: mockCreateTask,
    updateTask: mockUpdateTask,
    deleteTask: mockDeleteTask,
    moveTask: mockMoveTask,
  }),
}));

describe("useTasks", () => {
  beforeEach(() => {
    mockGetTasks.mockReset();
    mockCreateTask.mockReset();
    mockUpdateTask.mockReset();
    mockDeleteTask.mockReset();
    mockMoveTask.mockReset();

    mockGetTasks.mockResolvedValue([
      {
        id: "t1",
        columnId: "c1",
        name: "Task 1",
        description: "",
        icon: null,
        order: 1,
      },
    ]);
  });

  it("charge les tâches au montage", async () => {
    const { result } = renderHook(() => useTasks("123"));

    await waitFor(() => expect(result.current.loading).toBe(false));

    expect(mockGetTasks).toHaveBeenCalled();
    expect(result.current.tasks).toHaveLength(1);
  });

  it("createTask appelle le service et recharge", async () => {
    mockCreateTask.mockResolvedValue({ id: "t99" });

    const { result } = renderHook(() => useTasks("123"));

    await waitFor(() => expect(result.current.loading).toBe(false));

    await act(async () => {
      await result.current.createTask({
        name: "X",
        description: "",
        columnId: "c1",
      });
    });

    expect(mockCreateTask).toHaveBeenCalledWith("123", {
      name: "X",
      description: "",
      columnId: "c1",
    });

    expect(mockGetTasks).toHaveBeenCalledTimes(3);
  });

  it("updateTask appelle le service et recharge", async () => {
    mockUpdateTask.mockResolvedValue(undefined);

    const { result } = renderHook(() => useTasks("123"));

    await waitFor(() => expect(result.current.loading).toBe(false));

    await act(async () => {
      await result.current.updateTask("t1", {
        name: "Updated",
        description: "",
        columnId: "c1",
      });
    });

    expect(mockUpdateTask).toHaveBeenCalledWith("t1", {
      name: "Updated",
      description: "",
      columnId: "c1",
    });

    expect(mockGetTasks).toHaveBeenCalledTimes(3);
  });

  it("deleteTask appelle le service et recharge", async () => {
    mockDeleteTask.mockResolvedValue(undefined);

    const { result } = renderHook(() => useTasks("123"));

    await waitFor(() => expect(result.current.loading).toBe(false));

    await act(async () => {
      await result.current.deleteTask("t1");
    });

    expect(mockDeleteTask).toHaveBeenCalledWith("t1");
    expect(mockGetTasks).toHaveBeenCalledTimes(3);
  });

  it("moveTask appelle le service et recharge", async () => {
    mockMoveTask.mockResolvedValue(undefined);

    const { result } = renderHook(() => useTasks("123"));

    await waitFor(() => expect(result.current.loading).toBe(false));

    await act(async () => {
      await result.current.moveTask("t1", { columnId: "c2", order: 0 });
    });

    expect(mockMoveTask).toHaveBeenCalledWith("t1", {
      columnId: "c2",
      order: 0,
    });

    expect(mockGetTasks).toHaveBeenCalledTimes(3);
  });
});
