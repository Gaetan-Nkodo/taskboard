import { setupServer } from "msw/node";
import { http, HttpResponse } from "msw";

export const server = setupServer(
  // Health
  http.get("/health", () => HttpResponse.json({ status: "ok" })),

  // Boards
  http.get("*/api/v1/boards", () =>
    HttpResponse.json([
      { id: "1", name: "Board A" },
      { id: "2", name: "Board B" }
    ])
  ),

  // Tasks
  http.get("*/api/v1/tasks", () =>
    HttpResponse.json([
      { id: "1", title: "Task A" },
      { id: "2", title: "Task B" }
    ])
  ),

  // Login
  http.post("*/api/v1/auth/login", async () =>
    HttpResponse.json({
      accessToken: "ACCESS_TOKEN",
      refreshToken: "REFRESH_TOKEN",
      user: { id: "1", email: "test@test.com" }
    })
  ),

  // Forgot password
  http.post("*/api/v1/auth/forgot-password", async () => {
    return HttpResponse.json({});
  }),

  // Reset password
  http.post("*/api/v1/auth/reset-password", async ({ request }) => {
    const body = (await request.json()) as { token: string; newPassword: string };

    if (body.token === "BADTOKEN") {
      return HttpResponse.json({ error: "Invalid token" }, { status: 401 });
    }

    return HttpResponse.json({});
  }),
  
  // Change password
  http.post("*/api/v1/auth/change-password", async ({ request }) => {
    const body = (await request.json()) as {
      currentPassword: string;
      newPassword: string;
    };

    const auth = request.headers.get("authorization");
    if (!auth || !auth.startsWith("Bearer ")) {
      return HttpResponse.json({ error: "Unauthorized" }, { status: 401 });
    }

    if (body.currentPassword !== "OLD") {
      return HttpResponse.json({ error: "Bad password" }, { status: 400 });
    }

    return HttpResponse.json({ ok: true }, { status: 200 });
  })
);
