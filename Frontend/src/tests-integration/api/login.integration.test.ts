import { http } from "../../core/api/httpClient";
import { API } from "../../core/api/endpoints";
import type { LoginResponse } from "../../core/models/Auth";

test("login returns mocked tokens and user (MSW)", async () => {
  const result = await http<LoginResponse>(API.login, {
    method: "POST",
    body: JSON.stringify({
      email: "test@example.com",
      password: "P@ssw0rd!"
    })
  });

  // --- Access Token ---
  expect(result.accessToken).toBeDefined();
  expect(typeof result.accessToken).toBe("string");
  expect(result.accessToken.length).toBeGreaterThan(0); // MSW renvoie "fake-jwt"

  // --- Refresh Token ---
  expect(result.refreshToken).toBeDefined();
  expect(typeof result.refreshToken).toBe("string");

  // --- User ---
  expect(result.user).toBeDefined();
  expect(result.user.email).toBe("test@example.com");
});
