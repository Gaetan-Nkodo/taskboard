import { Navigate } from "react-router-dom";
import { useAuthContext } from "./AuthProvider";

export const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  const { user, accessToken, loading } = useAuthContext();

  if (loading) {
    return <div>Chargement...</div>;
  }

  if (!user || !accessToken) {
    return <Navigate to="/login" replace />;
  }

  return <>{children}</>;
};
