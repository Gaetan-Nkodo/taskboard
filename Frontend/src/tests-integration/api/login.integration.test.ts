import { http } from "../../core/api/httpClient";
import { API } from "../../core/api/endpoints";
import type { LoginResponse } from "../../core/models/Auth";

test("login returns token and user", async () => {
  const result = await http<LoginResponse>(API.login, {
    method: "POST",
    body: JSON.stringify({ email: "a", password: "b" })
  });

  expect(result.token).toBe("abc");
  expect(result.user).toBeDefined();
});
