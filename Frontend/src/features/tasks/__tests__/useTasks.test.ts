import { describe, it, expect, vi } from "vitest";
import { useTasks } from "../useTasks";
import * as httpModule from "../../../core/api/httpClient";

describe("useTasks", () => {
  it("getTasks appelle l’API", async () => {
    const spy = vi
      .spyOn(httpModule, "http")
      .mockResolvedValue([{ id: "1", title: "Test", status: "Todo", boardId: "b1" }]);

    const { getTasks } = useTasks();
    const tasks = await getTasks();

    expect(spy).toHaveBeenCalled();
    expect(tasks).toHaveLength(1);
  });

  it("createTask envoie les données", async () => {
    const spy = vi.spyOn(httpModule, "http").mockResolvedValue({
      id: "1",
      title: "New",
      status: "Todo",
      boardId: "b1"
    });

    const { createTask } = useTasks();
    const task = await createTask({ title: "New", boardId: "b1" });

    expect(spy).toHaveBeenCalled();
    expect(task.title).toBe("New");
  });
});
