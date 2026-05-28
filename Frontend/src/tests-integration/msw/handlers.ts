import { http, HttpResponse } from "msw";

const API_URL = "http://localhost";

export const handlers = [
  // --- Auth ---
  http.post(`${API_URL}/api/v1/auth/login`, async () => {
    return HttpResponse.json({
      token: "abc",
      user: { id: "1", email: "a@test.com", name: "Gaetan" }
    });
  }),

  // --- Boards ---
  http.get(`${API_URL}/api/v1/boards`, () => {
    return HttpResponse.json([
      { id: "b1", name: "Board A", description: "Desc A" },
      { id: "b2", name: "Board B", description: "Desc B" }
    ]);
  }),

  // --- Tasks ---
  http.get(`${API_URL}/api/v1/tasks`, () => {
    return HttpResponse.json([
      { id: "t1", title: "Task 1", boardId: "b1", columnId: "c1" },
      { id: "t2", title: "Task 2", boardId: "b1", columnId: "c1" }
    ]);
  }),

  // --- Health ---
  http.get(`${API_URL}/health`, () => {
    return HttpResponse.json({ status: "ok" });
  })
];
