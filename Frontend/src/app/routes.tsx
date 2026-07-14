import { createBrowserRouter } from "react-router-dom";

import LoginPage from "@/features/auth/LoginPage";
import ForgotPasswordPage from "@/features/auth/ForgotPasswordPage";
import ResetPasswordPage from "@/features/auth/ResetPasswordPage";
import { ChangePasswordPage } from "@/features/auth/ChangePasswordPage";
import RegisterPage from "@/features/auth/RegisterPage";
import ConfirmEmailPage from "@/features/auth/ConfirmEmailPage";
import ConfirmEmailSentPage from "@/features/auth/ConfirmEmailSentPage";

import { ProtectedRoute } from "@/features/auth/ProtectedRoute";
import { BoardsPage } from "@/features/boards/pages/BoardsPage";

import { Layout } from "@/components/layout/Layout";
import { BoardDetailsPage } from "@/features/boards/pages/BoardDetailsPage";
import { TasksPage } from "@/features/tasks/pages/TasksPage";

export const router = createBrowserRouter([
  // --- Pages publiques ---
  { path: "/login", element: <LoginPage /> },
  { path: "/register", element: <RegisterPage /> },
  { path: "/confirm-email", element: <ConfirmEmailPage /> },
  { path: "/confirm-email-sent", element: <ConfirmEmailSentPage /> },
  { path: "/forgot-password", element: <ForgotPasswordPage /> },
  { path: "/reset-password", element: <ResetPasswordPage /> },

  // --- Pages protégées ---
  {
    path: "/",
    element: (
      <ProtectedRoute>
        <Layout />
      </ProtectedRoute>
    ),
    children: [
      { index: true, element: <BoardsPage /> },
      { path: "boards", element: <BoardsPage /> },
      { path: "boards/:id", element: <BoardDetailsPage  /> },
      { path: "change-password", element: <ChangePasswordPage /> },
      { path: "tasks", element: <TasksPage /> }
    ]
  }
]);
