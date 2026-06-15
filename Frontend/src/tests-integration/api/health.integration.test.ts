import { rawHttp } from "@/core/api/rawHttp";

test("health endpoint returns ok", async () => {
  const result = await rawHttp<{ status: string }>("/health");
  expect(result.status).toBe("ok");
});
