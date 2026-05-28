import { http } from "../../core/api/httpClient";
import { API } from "../../core/api/endpoints";
import type { Board } from "../../core/models/Board";

test("boards endpoint returns a list", async () => {
  const result = await http<Board[]>(API.boards);

  expect(Array.isArray(result)).toBe(true);
  expect(result.length).toBe(2);
  expect(result[0].name).toBe("Board A");
});
