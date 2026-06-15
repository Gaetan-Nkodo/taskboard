import { rawHttp } from "@/core/api/rawHttp";
import { API } from "@/core/api/endpoints";

test("reset password succeeds with valid token", async () => {
  const result = await rawHttp(API.resetPassword, {
    method: "POST",
    body: JSON.stringify({ token: "GOOD", newPassword: "NEW" })
  });

  expect(result).toEqual({});
});

test("reset password fails with invalid token", async () => {
  await expect(
    rawHttp(API.resetPassword, {
      method: "POST",
      body: JSON.stringify({ token: "BADTOKEN", newPassword: "NEW" })
    })
  ).rejects.toThrow();
});
