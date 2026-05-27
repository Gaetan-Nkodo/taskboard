import { createBrowserRouter } from "react-router-dom";
import App from "./App";
import BoardPage from "../features/boards/BoardPage";
import LoginPage from "../features/auth/LoginPage";
import ProtectedRoute from "../features/auth/ProtectedRoute";

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
        <BoardPage />
      </ProtectedRoute>
    )
  }
]);
