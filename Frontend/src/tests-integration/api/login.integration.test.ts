import { rawHttp } from "../../core/api/rawHttp";
import { API } from "../../core/api/endpoints";
import type { LoginResponse } from "../../core/models/Auth";

test("login returns mocked tokens and user (MSW)", async () => {
  const result = await rawHttp<LoginResponse>(API.login, {
    method: "POST",
    body: JSON.stringify({
      email: "test@example.com",
      password: "P@ssw0rd!"
    })
  });

  expect(result).toEqual({
    accessToken: "ACCESS_TOKEN",
    refreshToken: "REFRESH_TOKEN",
    user: { id: "1", email: "test@test.com" }
  });
});
