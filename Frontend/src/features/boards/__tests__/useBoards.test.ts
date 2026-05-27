import { describe, it, expect, vi } from "vitest";
import { useBoards } from "../useBoards";
import * as httpModule from "../../../core/api/httpClient";

describe("useBoards", () => {
  it("getBoards appelle l’API", async () => {
    const spy = vi.spyOn(httpModule, "http").mockResolvedValue([]);

    const { getBoards } = useBoards();
    await getBoards();

    expect(spy).toHaveBeenCalled();
  });

  it("createBoard envoie les données", async () => {
    const spy = vi.spyOn(httpModule, "http").mockResolvedValue({
      id: "1",
      name: "Board A"
    });

    const { createBoard } = useBoards();
    const board = await createBoard({ name: "Board A" });

    expect(board.name).toBe("Board A");
  });
});
