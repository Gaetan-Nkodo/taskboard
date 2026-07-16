import { createBrowserRouter } from "react-router-dom";
import { lazy, Suspense } from "react";

import LoginPage from "@/features/auth/LoginPage";
import RegisterPage from "@/features/auth/RegisterPage";
import ForgotPasswordPage from "@/features/auth/ForgotPasswordPage";
import ResetPasswordPage from "@/features/auth/ResetPasswordPage";
import ConfirmEmailPage from "@/features/auth/ConfirmEmailPage";
import ConfirmEmailSentPage from "@/features/auth/ConfirmEmailSentPage";
import { ChangePasswordPage } from "@/features/auth/ChangePasswordPage";

import { ProtectedRoute } from "@/features/auth/ProtectedRoute";
import { Layout } from "@/components/layout/Layout";

import { DashboardPage } from "@/features/dashboard/DashboardPage";

// Lazy pages (must have export default)
const BoardsPage = lazy(() => import("@/features/boards/pages/BoardsPage"));
const BoardDetailsPage = lazy(() => import("@/features/boards/pages/BoardDetailsPage"));
const TasksPage = lazy(() => import("@/features/tasks/pages/TasksPage"));

// Wrapper to avoid repeating Suspense everywhere
const Lazy = (Component: React.LazyExoticComponent<any>) => (
  <Suspense fallback={<div className="p-6">Chargement…</div>}>
    <Component />
  </Suspense>
);

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
      { index: true, element: <DashboardPage /> },

      { path: "boards", element: Lazy(BoardsPage) },
      { path: "boards/:id", element: Lazy(BoardDetailsPage) },

      { path: "tasks", element: Lazy(TasksPage) },

      { path: "change-password", element: <ChangePasswordPage /> }
    ]
  }
]);
