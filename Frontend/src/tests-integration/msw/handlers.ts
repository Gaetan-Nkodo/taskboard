import { http, HttpResponse } from "msw";

const API_URL = "http://localhost";

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
  http.post(`${API_URL}/api/v1/auth/register`, async ({ request }) => {
    const body = (await request.json()) as RegisterBody;

    return HttpResponse.json({
      id: "fake-id",
      email: body.email,
      fullName: body.fullName
    });
  }),

  // --- Auth: Login ---
  http.post(`${API_URL}/api/v1/auth/login`, async ({ request }) => {
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
