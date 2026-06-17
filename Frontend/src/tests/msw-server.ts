import { setupServer } from "msw/node";
import { http, HttpResponse } from "msw";

export let boards = [
  { id: "1", name: "Board A", description: "Desc A", columns: [] },
  { id: "2", name: "Board B", description: "Desc B", columns: [] },
];

export const server = setupServer(

  // -----------------------------
  // HEALTH
  // -----------------------------
  http.get("/health", () =>
    HttpResponse.json({ status: "ok" })
  ),

  // -----------------------------
  // BOARDS
  // -----------------------------
  http.get("/api/v1/boards", () => {
    return HttpResponse.json(boards);
  }),

  http.post("/api/v1/boards", async ({ request }) => {
    const body = (await request.clone().json()) as {
      name?: string;
      description?: string | null;
    };

    if (!body || typeof body !== "object") {
      return HttpResponse.json({ error: "Invalid body" }, { status: 400 });
    }

    const newBoard = {
      id: crypto.randomUUID(),
      name: body.name ?? "",
      description: body.description ?? "",
      columns: [],
    };

    boards.push(newBoard);
    return HttpResponse.json(newBoard);
  }),

  http.put("/api/v1/boards/:id", async ({ params, request }) => {
    const body = (await request.clone().json()) as {
      name?: string;
      description?: string | null;
    };

    if (!body || typeof body !== "object") {
      return HttpResponse.json({ error: "Invalid body" }, { status: 400 });
    }

    const index = boards.findIndex((b) => b.id === params.id);
    if (index !== -1) {
      boards[index] = {
        ...boards[index],
        ...body,
        description: body.description ?? boards[index].description,
      };
    }

    return HttpResponse.json({});
  }),

  http.delete("/api/v1/boards/:id", ({ params }) => {
    boards = boards.filter((b) => b.id !== params.id);
    return HttpResponse.json({});
  }),

  // -----------------------------
  // TASKS
  // -----------------------------
  http.get("/api/v1/tasks", () =>
    HttpResponse.json([
      { id: "1", title: "Task A" },
      { id: "2", title: "Task B" },
    ])
  ),

  // -----------------------------
  // AUTH
  // -----------------------------
  http.post("/api/v1/auth/login", async () =>
    HttpResponse.json({
      accessToken: "ACCESS_TOKEN",
      refreshToken: "REFRESH_TOKEN",
      user: { id: "1", email: "test@test.com", displayName: "Gaétan" },
    })
  ),

  http.post("/api/v1/auth/forgot-password", () =>
    HttpResponse.json({})
  ),

  http.post("/api/v1/auth/reset-password", async ({ request }) => {
    const body = (await request.clone().json()) as {
      token?: string;
      newPassword?: string;
    };

    if (!body || typeof body !== "object") {
      return HttpResponse.json({ error: "Invalid body" }, { status: 400 });
    }

    if (body.token === "BADTOKEN") {
      return HttpResponse.json({ error: "Invalid token" }, { status: 401 });
    }

    return HttpResponse.json({});
  }),

  http.post("/api/v1/auth/change-password", async ({ request }) => {
    const body = (await request.clone().json()) as {
      currentPassword?: string;
      newPassword?: string;
    };

    if (!body || typeof body !== "object") {
      return HttpResponse.json({ error: "Invalid body" }, { status: 400 });
    }

    const auth = request.headers.get("authorization");

    if (!auth?.startsWith("Bearer ")) {
      return HttpResponse.json({ error: "Unauthorized" }, { status: 401 });
    }

    if (body.currentPassword !== "OLD") {
      return HttpResponse.json({ error: "Bad password" }, { status: 400 });
    }

    return HttpResponse.json({ ok: true });
  })
);
