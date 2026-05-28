import { http } from "../../core/api/httpClient";
import { API } from "../../core/api/endpoints";

test("health endpoint responds", async () => {
  const result = await http(API.health);

  expect(result).toEqual({ status: "ok" });
});
