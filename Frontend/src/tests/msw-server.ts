import { setupServer } from "msw/node";
import { http, HttpResponse } from "msw";

export const server = setupServer(
  http.get("/health", () => HttpResponse.json({ status: "ok" })),

  http.get("*/api/v1/boards", () =>
    HttpResponse.json([
      { id: "1", name: "Board A" },
      { id: "2", name: "Board B" }
    ])
  ),

  http.get("*/api/v1/tasks", () =>
    HttpResponse.json([
      { id: "1", title: "Task A" },
      { id: "2", title: "Task B" }
    ])
  ),

  http.post("*/api/v1/auth/login", async () =>
    HttpResponse.json({
      accessToken: "ACCESS_TOKEN",
      refreshToken: "REFRESH_TOKEN",
      user: { id: "1", email: "test@test.com" }
    })
  )
);
