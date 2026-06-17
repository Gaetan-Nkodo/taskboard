import { describe, it, expect, beforeEach, vi } from "vitest";
import { createBoardService } from "../BoardService";

describe("BoardService", () => {
  const mockHttp = vi.fn();
  let service: ReturnType<typeof createBoardService>;

  beforeEach(() => {
    mockHttp.mockReset();
    service = createBoardService(mockHttp);
  });

  it("getBoards appelle GET /api/v1/boards", async () => {
    mockHttp.mockResolvedValue([]);
    await service.getBoards();
    expect(mockHttp).toHaveBeenCalledWith("/api/v1/boards");
  });

  it("getBoard appelle GET /api/v1/boards/:id", async () => {
    mockHttp.mockResolvedValue({ id: "b1" });
    await service.getBoard("b1");
    expect(mockHttp).toHaveBeenCalledWith("/api/v1/boards/b1");
  });

  it("createBoard appelle POST /api/v1/boards", async () => {
    mockHttp.mockResolvedValue({ id: "new-id" });
    await service.createBoard({ name: "Test", description: "" });
    expect(mockHttp).toHaveBeenCalledWith("/api/v1/boards", {
      method: "POST",
      body: JSON.stringify({ name: "Test", description: "" }),
    });
  });

  it("updateBoard appelle PUT /api/v1/boards/:id", async () => {
    mockHttp.mockResolvedValue(undefined);
    await service.updateBoard("b1", { name: "Updated", description: "" });
    expect(mockHttp).toHaveBeenCalledWith("/api/v1/boards/b1", {
      method: "PUT",
      body: JSON.stringify({ name: "Updated", description: "" }),
    });
  });

  it("deleteBoard appelle DELETE /api/v1/boards/:id", async () => {
    mockHttp.mockResolvedValue(undefined);
    await service.deleteBoard("b1");
    expect(mockHttp).toHaveBeenCalledWith("/api/v1/boards/b1", {
      method: "DELETE",
    });
  });
});
