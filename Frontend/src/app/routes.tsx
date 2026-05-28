import { createBrowserRouter } from "react-router-dom";
import App from "./App";
import LoginPage from "../features/auth/LoginPage";
import { ProtectedRoute } from "../features/auth/ProtectedRoute";
import { BoardsPage } from "../features/boards/BoardsPage";


export const router = createBrowserRouter([
  { path: "/login", element: <LoginPage /> },

  {
    path: "/",
    element: (
      <ProtectedRoute>
        <App />
      </ProtectedRoute>
    )
  },

  {
    path: "/boards/:id",
    element: (
      <ProtectedRoute>
        <BoardsPage />
      </ProtectedRoute>
    )
  }
]);
