import { describe, it, expect, vi, beforeEach } from "vitest";
import { createBoardService } from "../BoardService";
import type { CreateBoardRequest, UpdateBoardRequest } from "../../types/BoardTypes";

describe("BoardService", () => {
  let http: ReturnType<typeof vi.fn>;
  let service: ReturnType<typeof createBoardService>;

  beforeEach(() => {
    http = vi.fn();
    service = createBoardService(http as any);
  });

  it("getBoards appelle GET /api/v1/boards", async () => {
    http.mockResolvedValue([]);
    await service.getBoards();

    expect(http).toHaveBeenCalledWith("/api/v1/boards");
  });

  it("getBoard appelle GET /api/v1/boards/:id", async () => {
    http.mockResolvedValue({ id: "b1" });
    await service.getBoard("b1");

    expect(http).toHaveBeenCalledWith("/api/v1/boards/b1");
  });

  it("createBoard appelle POST /api/v1/boards", async () => {
    const payload: CreateBoardRequest = {
      name: "Test",
      description: "",
    };

    http.mockResolvedValue({ id: "new-id" });

    await service.createBoard(payload);

    expect(http).toHaveBeenCalledWith("/api/v1/boards", {
      method: "POST",
      body: JSON.stringify(payload),
    });
  });

  it("updateBoard appelle PUT /api/v1/boards/:id", async () => {
    const payload: UpdateBoardRequest = {
      name: "Updated",
      description: "",
    };

    http.mockResolvedValue(undefined);

    await service.updateBoard("b1", payload);

    expect(http).toHaveBeenCalledWith("/api/v1/boards/b1", {
      method: "PUT",
      body: JSON.stringify(payload),
    });
  });

  it("deleteBoard appelle DELETE /api/v1/boards/:id", async () => {
    http.mockResolvedValue(undefined);

    await service.deleteBoard("b1");

    expect(http).toHaveBeenCalledWith("/api/v1/boards/b1", {
      method: "DELETE",
    });
  });
});
