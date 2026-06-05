import { setupServer } from "msw/node";
import { http, HttpResponse } from "msw";

// 🟦 MSW SERVER
export const server = setupServer(
  http.get("*/health", () => {
    return HttpResponse.json({ status: "Healthy" });
  }),

  http.get("*/api/tasks", () =>
    HttpResponse.json([
      { id: "1", title: "Task A", status: "Todo", boardId: "b1" },
      { id: "2", title: "Task B", status: "Done", boardId: "b1" }
    ])
  ),

  http.get("*/api/boards/1", () =>
    HttpResponse.json({ id: "1", name: "Board A", description: "Demo" })
  ),

  http.post("*/api/auth/login", async () =>
    HttpResponse.json({
      token: "abc",
      user: { id: "1", email: "test@test.com", name: "Gaetan" }
    })
  )
);

// 🟦 LIFECYCLE
beforeAll(() => server.listen());
afterEach(() => server.resetHandlers());
afterAll(() => server.close());
