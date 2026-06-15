import { rawHttp } from "@/core/api/rawHttp";
import { API } from "@/core/api/endpoints";
import type { LoginResponse } from "@/core/models/Auth";

test("login returns tokens and user", async () => {
  const result = await rawHttp<LoginResponse>(API.login, {
    method: "POST",
    body: JSON.stringify({ email: "test@test.com", password: "pwd" })
  });

  expect(result.accessToken).toBe("ACCESS_TOKEN");
  expect(result.refreshToken).toBe("REFRESH_TOKEN");
  expect(result.user.email).toBe("test@test.com");
});
