export const API = {
  // --- Health ---
  health: "/health",

  // --- Auth ---
  login: "/api/v1/auth/login",
  register: "/api/v1/auth/register",
  refresh: "/api/v1/auth/refresh",
  logout: "/api/v1/auth/logout",
  changePassword: "/api/v1/auth/change-password",

  // --- Boards ---
  boards: "/api/v1/boards",
  board: (id: string) => `/api/v1/boards/${id}`,

  // --- Tasks ---
  tasks: "/api/v1/tasks",
  task: (id: string) => `/api/v1/tasks/${id}`,

  // --- Password Reset ---
  resetPassword: "/api/v1/auth/reset-password"
} as const;
