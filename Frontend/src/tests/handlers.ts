import { http, HttpResponse } from "msw";

type RegisterBody = {
  email: string;
  password: string;
  fullName: string;
};

type LoginBody = {
  email: string;
  password: string;
};

type ChangePasswordBody = {
  currentPassword: string;
  newPassword: string;
};

export const handlers = [
  // -----------------------------
  // AUTH
  // -----------------------------
  http.post("/api/v1/auth/register", async ({ request }) => {
    const body = (await request.json()) as RegisterBody;

    return HttpResponse.json({
      id: "fake-id",
      email: body.email,
      fullName: body.fullName
    });
  }),

  http.post("/api/v1/auth/login", async ({ request }) => {
    const body = (await request.json()) as LoginBody;

    return HttpResponse.json({
      accessToken: "fake-jwt",
      refreshToken: "fake-refresh",
      user: {
        id: "fake-id",
        email: body.email,
        fullName: "Test User"
      }
    });
  }),

  http.post("/api/v1/auth/refresh", () =>
    HttpResponse.json({
      accessToken: "new-token",
      refreshToken: "new-refresh",
      user: {
        id: "fake-id",
        email: "test@test.com",
        fullName: "Test User"
      }
    })
  ),

  http.post("/api/v1/auth/forgot-password", () =>
    HttpResponse.json({ ok: true })
  ),

  http.post("/api/v1/auth/reset-password", () =>
    HttpResponse.json({ ok: true })
  ),

  http.post("/api/v1/auth/change-password", async ({ request }) => {
    const body = (await request.json()) as ChangePasswordBody;

    const auth = request.headers.get("authorization");
    if (!auth?.startsWith("Bearer ")) {
      return HttpResponse.json({ error: "Unauthorized" }, { status: 401 });
    }

    if (body.currentPassword !== "OLD") {
      return HttpResponse.json({ error: "Bad password" }, { status: 400 });
    }

    return HttpResponse.json({ ok: true });
  }),

  // -----------------------------
  // BOARDS
  // -----------------------------
  http.get("/api/v1/boards", () =>
    HttpResponse.json([
      { id: "b1", name: "Board A", description: "Desc A" },
      { id: "b2", name: "Board B", description: "Desc B" }
    ])
  ),

  // -----------------------------
  // TASKS
  // -----------------------------
  http.get("/api/v1/tasks", () =>
    HttpResponse.json([
      { id: "t1", title: "Task 1", boardId: "b1", columnId: "c1" },
      { id: "t2", title: "Task 2", boardId: "b1", columnId: "c1" }
    ])
  ),

  // -----------------------------
  // HEALTH
  // -----------------------------
  http.get("/health", () =>
    HttpResponse.json({ status: "ok" })
  )
];
