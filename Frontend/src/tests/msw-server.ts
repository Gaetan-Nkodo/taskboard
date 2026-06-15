import { setupServer } from "msw/node";
import { http, HttpResponse } from "msw";

export const server = setupServer(
  http.get(/\/health$/, () =>
    HttpResponse.json({ status: "ok" })
  ),

  http.get(/\/boards$/, () =>
    HttpResponse.json([
      { id: "1", name: "Board A" },
      { id: "2", name: "Board B" }
    ])
  ),

  http.get(/\/tasks$/, () =>
    HttpResponse.json([
      { id: "1", title: "Task A" },
      { id: "2", title: "Task B" }
    ])
  ),

  http.post(/\/auth\/login$/, async () =>
    HttpResponse.json({
      accessToken: "ACCESS_TOKEN",
      refreshToken: "REFRESH_TOKEN",
      user: { id: "1", email: "test@test.com", displayName: "Gaétan" }
    })
  ),

  http.post(/\/auth\/forgot-password$/, async () =>
    HttpResponse.json({})
  ),

  http.post(/\/auth\/reset-password$/, async ({ request }) => {
    const body = (await request.json()) as { token?: string; newPassword?: string };

    if (body.token === "BADTOKEN") {
      return HttpResponse.json({ error: "Invalid token" }, { status: 401 });
    }

    return HttpResponse.json({});
  }),

  http.post(/\/auth\/change-password$/, async ({ request }) => {
    const body = (await request.json()) as {
      currentPassword?: string;
      newPassword?: string;
    };

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
