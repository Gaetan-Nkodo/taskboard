import { describe, it, expect, vi, beforeEach } from "vitest";
import { createTaskService } from "../TaskService";
import type { CreateTaskRequest, UpdateTaskRequest } from "../../types/TaskTypes";

describe("TaskService", () => {
  let http: ReturnType<typeof vi.fn>;
  let service: ReturnType<typeof createTaskService>;

  beforeEach(() => {
    http = vi.fn();
    service = createTaskService(http as any);
  });

  it("getTasks appelle GET /api/v1/boards/{id}/tasks", async () => {
    http.mockResolvedValue([]);
    await service.getTasks("123");

    expect(http).toHaveBeenCalledWith("/api/v1/boards/123/tasks");
  });

  it("createTask appelle POST /api/v1/boards/{id}/tasks", async () => {
    const payload: CreateTaskRequest = {
      name: "Task X",
      columnId: "c1",
      description: "",
    };

    http.mockResolvedValue({ id: "t99" });

    await service.createTask("123", payload);

    expect(http).toHaveBeenCalledWith("/api/v1/boards/123/tasks", {
      method: "POST",
      body: JSON.stringify(payload),
    });
  });

  it("updateTask appelle PUT /api/v1/tasks/{id}", async () => {
    const payload: UpdateTaskRequest = {
      name: "Updated",
      description: "",
      columnId: "c1",
    };

    http.mockResolvedValue(undefined);

    await service.updateTask("t1", payload);

    expect(http).toHaveBeenCalledWith("/api/v1/tasks/t1", {
      method: "PUT",
      body: JSON.stringify(payload),
    });
  });

  it("deleteTask appelle DELETE /api/v1/tasks/{id}", async () => {
    http.mockResolvedValue(undefined);

    await service.deleteTask("t1");

    expect(http).toHaveBeenCalledWith("/api/v1/tasks/t1", {
      method: "DELETE",
    });
  });

  it("moveTask appelle PATCH /api/v1/tasks/{id}/move", async () => {
    http.mockResolvedValue(undefined);

    await service.moveTask("t1", { columnId: "c2", order: 0 });

    expect(http).toHaveBeenCalledWith("/api/v1/tasks/t1/move", {
      method: "PATCH",
      body: JSON.stringify({ columnId: "c2", order: 0 }),
    });
  });
});
