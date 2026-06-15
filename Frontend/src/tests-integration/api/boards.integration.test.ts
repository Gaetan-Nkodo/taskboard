import { rawHttp } from "@/core/api/rawHttp";
import { API } from "@/core/api/endpoints";
import type { Board } from "@/core/models/Board";

test("boards endpoint returns a list", async () => {
  const result = await rawHttp<Board[]>(API.boards);

  expect(result.length).toBe(2);
  expect(result[0].name).toBe("Board A");
});
