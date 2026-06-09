import { createBrowserRouter } from "react-router-dom";
import App from "./App";
import LoginPage from "../features/auth/LoginPage";
import { ProtectedRoute } from "../features/auth/ProtectedRoute";
import { BoardsPage } from "../features/boards/BoardsPage";
import ForgotPasswordPage from "../features/auth/ForgotPasswordPage";
import ResetPasswordPage from "../features/auth/ResetPasswordPage";

export const router = createBrowserRouter([
  { path: "/login", element: <LoginPage /> },
  { path: "/forgot-password", element: <ForgotPasswordPage /> },
  { path: "/reset-password", element: <ResetPasswordPage /> },

  {
    path: "/",
    element: (
      <ProtectedRoute>
        <App />
      </ProtectedRoute>
    ),
    children: [
      { index: true, element: <BoardsPage /> },
      { path: "boards", element: <BoardsPage /> },
      { path: "boards/:id", element: <BoardsPage /> }
    ]
  }
]);
