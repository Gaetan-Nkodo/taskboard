import { http } from "../../core/api/httpClient";
import { API } from "../../core/api/endpoints";
import type { Task } from "../../core/models/Task";

test("tasks endpoint returns a list", async () => {
  const result = await http<Task[]>(API.tasks);
  expect(Array.isArray(result)).toBe(true);
  expect(result.length).toBeGreaterThan(0);
});
