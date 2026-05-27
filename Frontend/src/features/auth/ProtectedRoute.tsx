import { Navigate, useLocation } from "react-router-dom";
import { useAuthContext } from "./AuthContext";
import type { ReactNode } from "react";

export default function ProtectedRoute({ children }: { children: ReactNode }) {
  const { user } = useAuthContext();
  const location = useLocation();

  if (!user) {
    return (
      <Navigate
        to="/login"
        replace
        state={{ from: location.pathname }} // 🔥 returnTo
      />
    );
  }

  return children;
}
