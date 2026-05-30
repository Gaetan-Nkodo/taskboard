export const API = {
  // --- Health ---
  health: "/health",

  // --- Auth ---
  login: "/api/v1/auth/login",
  register: "/api/v1/auth/register",

  // --- Boards ---
  boards: "/api/v1/boards",
  board: (id: string) => `/api/v1/boards/${id}`,

  // --- Tasks ---
  tasks: "/api/v1/tasks",
  task: (id: string) => `/api/v1/tasks/${id}`
} as const;
