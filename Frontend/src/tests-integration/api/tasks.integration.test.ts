import { rawHttp } from "@/core/api/rawHttp";
import { API } from "@/core/api/endpoints";
import type { Task } from "@/core/models/Task";

test("tasks endpoint returns a list", async () => {
  const result = await rawHttp<Task[]>(API.tasks);

  expect(result.length).toBe(2);
  expect(result[0].title).toBe("Task A");
});
