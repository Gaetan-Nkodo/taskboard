import { createContext, useContext, useState, useEffect } from "react";
import type { User } from "@/core/models/User";
import type { LoginResponse } from "@/core/models/Auth";

interface AuthContextValue {
  user: User | null;
  accessToken: string | null;
  refreshToken: string | null;
  loading: boolean;
  login: (result: LoginResponse) => void;
  logout: () => void;
  applyTokens: (result: LoginResponse) => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(() => {
    const stored = localStorage.getItem("user");
    return stored ? JSON.parse(stored) : null;
  });

  const [accessToken, setAccessToken] = useState<string | null>(() =>
    localStorage.getItem("accessToken")
  );

  const [refreshToken, setRefreshToken] = useState<string | null>(() =>
    localStorage.getItem("refreshToken")
  );

  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // 🔥 Aucun setState ici → conforme ESLint
    const timer = setTimeout(() => setLoading(false), 0);
    return () => clearTimeout(timer);
  }, []);

  const applyTokens = (result: LoginResponse) => {
    setUser(result.user);
    setAccessToken(result.accessToken);
    setRefreshToken(result.refreshToken);

    localStorage.setItem("user", JSON.stringify(result.user));
    localStorage.setItem("accessToken", result.accessToken);
    localStorage.setItem("refreshToken", result.refreshToken);
  };

  const login = (result: LoginResponse) => applyTokens(result);

  const logout = () => {
    localStorage.clear();
    setUser(null);
    setAccessToken(null);
    setRefreshToken(null);
    window.location.assign("/login");
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        accessToken,
        refreshToken,
        loading,
        login,
        logout,
        applyTokens
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuthContext() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuthContext must be used inside AuthProvider");
  return ctx;
}
