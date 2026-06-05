import { rawHttp } from "../../core/api/rawHttp";
import { API } from "../../core/api/endpoints";

test("health endpoint responds", async () => {
  const result = await rawHttp<{ status: string }>(API.health);

  expect(result).toEqual({ status: "ok" });
});
