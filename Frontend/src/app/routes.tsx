import { createBrowserRouter } from "react-router-dom";

import LoginPage from "@/features/auth/LoginPage";
import ForgotPasswordPage from "@/features/auth/ForgotPasswordPage";
import ResetPasswordPage from "@/features/auth/ResetPasswordPage";
import { ChangePasswordPage } from "@/features/auth/ChangePasswordPage";

import { ProtectedRoute } from "@/features/auth/ProtectedRoute";
import { BoardsPage } from "@/features/boards/BoardsPage";

import { Layout } from "@/components/layout/Layout";

export const router = createBrowserRouter([
  // --- Pages publiques ---
  { path: "/login", element: <LoginPage /> },
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
      { path: "boards/:id", element: <BoardsPage /> },
      { path: "change-password", element: <ChangePasswordPage /> }
    ]
  }
]);
