import { http } from "../../core/api/httpClient";
import { API } from "../../core/api/endpoints";
import type { Board } from "../../core/models/Board";

test("boards endpoint returns a list", async () => {
  const result = await http<Board[]>(API.boards);
  expect(result.length).toBe(2);
});
