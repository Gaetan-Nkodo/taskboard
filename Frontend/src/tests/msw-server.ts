import { setupServer } from "msw/node";
import { http, HttpResponse } from "msw";
import type { TaskDto } from "@/features/tasks/types/TaskTypes";

// -----------------------------------------------------------------------------
// MOCK DATA
// -----------------------------------------------------------------------------
export let boards = [
  {
    id: "1",
    name: "Board A",
    description: "Desc A",
    columns: [
      { id: "backlog", name: "Backlog", order: 1 },
      { id: "ready", name: "Ready", order: 2 },
      { id: "inprogress", name: "In Progress", order: 3 },
      { id: "review", name: "Review", order: 4 },
      { id: "done", name: "Done", order: 5 }
    ]
  },
  {
    id: "2",
    name: "Board B",
    description: "Desc B",
    columns: []
  }
];

export let tasks: TaskDto[] = [
  {
    id: "t1",
    boardId: "1",
    name: "Task 1",
    description: "Task 1 desc",
    icon: null,
    columnId: "backlog",
    order: 1,
    assigneeId: null,
    dueDate: null,
    priority: null,
    createdAt: '',
  },
  {
    id: "t2",
    boardId: "1",
    name: "Task 2",
    description: "Task 2 desc",
    icon: null,
    columnId: "backlog",
    order: 2,
    assigneeId: null,
    dueDate: null,
    priority: null,
    createdAt: ''
  }
];

const urls = (path: string) => [
  path,
  `http://localhost${path}`,
  `http://localhost:3000${path}`
];

// -----------------------------------------------------------------------------
// SERVER
// -----------------------------------------------------------------------------
export const server = setupServer(

  // HEALTH
  ...urls("/health").map(url =>
    http.get(url, () => HttpResponse.json({ status: "ok" }))
  ),

  // ---------------------------------------------------------------------------
  // BOARDS
  // ---------------------------------------------------------------------------
  ...urls("/api/v1/boards").map(url =>
    http.get(url, () =>
      HttpResponse.json(
        boards.map(b => ({
          id: b.id,
          name: b.name,
          description: b.description,
          columns: b.columns
        }))
      )
    )
  ),

  ...urls("/api/v1/boards/:id").map(url =>
    http.get(url, ({ params }) => {
      const board = boards.find(b => b.id === params.id);
      if (!board) return HttpResponse.json({ message: "Not found" }, { status: 404 });
      return HttpResponse.json(board);
    })
  ),

  ...urls("/api/v1/boards").map(url =>
    http.post(url, async ({ request }) => {
      const body = (await request.json()) as {
        name?: string;
        description?: string | null;
      };

      const newBoard = {
        id: crypto.randomUUID(),
        name: body.name ?? "",
        description: body.description ?? "",
        columns: []
      };

      boards.push(newBoard);
      return HttpResponse.json(newBoard);
    })
  ),

  ...urls("/api/v1/boards/:id").map(url =>
    http.put(url, async ({ params, request }) => {
      const body = (await request.json()) as {
        name?: string;
        description?: string | null;
      };

      const index = boards.findIndex(b => b.id === params.id);

      if (index !== -1) {
        boards[index] = {
          ...boards[index],
          ...body,
          description: body.description ?? boards[index].description
        };
      }

      return HttpResponse.json({});
    })
  ),

  ...urls("/api/v1/boards/:id").map(url =>
    http.delete(url, ({ params }) => {
      boards = boards.filter(b => b.id !== params.id);
      return HttpResponse.json({});
    })
  ),

  // ---------------------------------------------------------------------------
  // TASKS
  // ---------------------------------------------------------------------------
  // 🔥 Version STABLE pour tous les tests
  ...urls("/api/v1/boards/:id/tasks").map(url =>
    http.get(url, () =>
      HttpResponse.json([
        { id: "t1", name: "Task 1", columnId: "backlog", order: 0 },
        { id: "t2", name: "Task 2", columnId: "backlog", order: 1 }
      ])
    )
  ),

  ...urls("/api/v1/boards/:id/tasks").map(url =>
    http.post(url, async ({ params, request }) => {
      const boardId = params.id as string;

      const body = (await request.json()) as {
        name: string;
        description?: string | null;
        columnId: string;
        icon?: string | null;
      };

      const newTask: TaskDto = {
        id: crypto.randomUUID(),
        boardId,
        name: body.name,
        description: body.description ?? null,
        icon: body.icon ?? null,
        columnId: body.columnId,
        order: tasks.length + 1,
        assigneeId: null,
        dueDate: null,
        priority: null,
        createdAt: new Date().toISOString()
      };

      tasks.push(newTask);
      return HttpResponse.json(newTask);
    })
  ),

  ...urls("/api/v1/tasks/:id").map(url =>
    http.put(url, async ({ params, request }) => {
      const body = (await request.json()) as Partial<TaskDto>;

      const index = tasks.findIndex(t => t.id === params.id);

      if (index !== -1) {
        tasks[index] = { ...tasks[index], ...body };
      }

      return HttpResponse.json({});
    })
  ),

  ...urls("/api/v1/tasks/:id").map(url =>
    http.delete(url, ({ params }) => {
      tasks = tasks.filter(t => t.id !== params.id);
      return HttpResponse.json({});
    })
  ),

  ...urls("/api/v1/tasks/:id/move").map(url =>
    http.patch(url, async ({ params, request }) => {
      const body = (await request.json()) as {
        columnId: string;
        order: number;
      };

      const index = tasks.findIndex(t => t.id === params.id);

      if (index === -1) {
        return HttpResponse.json({ message: "Not found" }, { status: 404 });
      }

      tasks[index].columnId = body.columnId;
      tasks[index].order = body.order;

      return HttpResponse.json({});
    })
  ),

  // ---------------------------------------------------------------------------
  // AUTH
  // ---------------------------------------------------------------------------
  ...urls("/api/v1/auth/login").map(url =>
    http.post(url, () =>
      HttpResponse.json({
        accessToken: "ACCESS_TOKEN",
        refreshToken: "REFRESH_TOKEN",
        user: { id: "1", email: "test@test.com", displayName: "Gaétan" }
      })
    )
  ),

  // FORGOT PASSWORD
  ...urls("/api/v1/auth/forgot-password").map(url =>
    http.post(url, async ({ request }) => {
      const body = (await request.json()) as { email: string };

      if (body.email === "test@example.com") {
        return HttpResponse.json({ success: true });
      }

      return HttpResponse.json({ message: "Email not found" }, { status: 404 });
    })
  ),

  // RESET PASSWORD — 🔥 renvoie {} comme attendu par les tests
  ...urls("/api/v1/auth/reset-password").map(url =>
    http.post(url, async ({ request }) => {
      const body = (await request.json()) as {
        token: string;
        newPassword: string;
      };

      if (body.token === "VALID" || body.token === "GOOD") {
        return HttpResponse.json({});
      }

      return HttpResponse.json({ message: "Invalid token" }, { status: 400 });
    })
  )
);
