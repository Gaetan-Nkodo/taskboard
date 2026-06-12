import { http, HttpResponse } from "msw";

// Types
type RegisterBody = {
  email: string;
  password: string;
  fullName: string;
};

type LoginBody = {
  email: string;
  password: string;
};

export const handlers = [
  // --- Auth: Register ---
  http.post("/api/v1/auth/register", async ({ request }) => {
    const body = (await request.json()) as RegisterBody;

    return HttpResponse.json({
      id: "fake-id",
      email: body.email,
      fullName: body.fullName
    });
  }),

  // --- Auth: Login ---
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

  // --- Auth: Refresh ---
  http.post("/api/v1/auth/refresh", async () => {
    return HttpResponse.json({
      accessToken: "new-token",
      refreshToken: "new-refresh",
      user: {
        id: "fake-id",
        email: "test@test.com",
        fullName: "Test User"
      }
    });
  }),

  // --- Auth: Forgot Password ---
  http.post("/api/v1/auth/forgot-password", async () => {
    return HttpResponse.json({ ok: true });
  }),

  // --- Auth: Reset Password ---
  http.post("/api/v1/auth/reset-password", async () => {
    return HttpResponse.json({ ok: true });
  }),

  // --- Auth: Change Password ---
  http.post("/api/v1/auth/change-password", async ({ request }) => {
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
  }),

  // --- Boards ---
  http.get("/api/v1/boards", () => {
    return HttpResponse.json([
      { id: "b1", name: "Board A", description: "Desc A" },
      { id: "b2", name: "Board B", description: "Desc B" }
    ]);
  }),

  // --- Tasks ---
  http.get("/api/v1/tasks", () => {
    return HttpResponse.json([
      { id: "t1", title: "Task 1", boardId: "b1", columnId: "c1" },
      { id: "t2", title: "Task 2", boardId: "b1", columnId: "c1" }
    ]);
  }),

  // --- Health ---
  http.get("/health", () => {
    return HttpResponse.json({ status: "ok" });
  })
];
