import { createContext, useContext, useEffect, useState } from "react";
import type { User } from "@/core/models/User";
import type { LoginResponse } from "@/core/models/Auth";

interface AuthContextValue {
  user: User | null;
  accessToken: string | null;
  refreshToken: string | null;
  loading: boolean;

  setUser: (u: User | null) => void;
  setAccessToken: (t: string | null) => void;
  setRefreshToken: (t: string | null) => void;

  logout: () => void;

  applyTokens: (result: LoginResponse) => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [accessToken, setAccessToken] = useState<string | null>(null);
  const [refreshToken, setRefreshToken] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const storedUser = localStorage.getItem("user");
    const storedAccess = localStorage.getItem("accessToken");
    const storedRefresh = localStorage.getItem("refreshToken");

    if (storedUser) setUser(JSON.parse(storedUser));
    if (storedAccess) setAccessToken(storedAccess);
    if (storedRefresh) setRefreshToken(storedRefresh);

    setLoading(false);
  }, []);

  const logout = () => {
    localStorage.removeItem("user");
    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");

    setUser(null);
    setAccessToken(null);
    setRefreshToken(null);
  };

  const applyTokens = (result: LoginResponse) => {
    setUser(result.user);
    setAccessToken(result.accessToken);
    setRefreshToken(result.refreshToken);

    localStorage.setItem("user", JSON.stringify(result.user));
    localStorage.setItem("accessToken", result.accessToken);
    localStorage.setItem("refreshToken", result.refreshToken);
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        accessToken,
        refreshToken,
        loading,
        setUser,
        setAccessToken,
        setRefreshToken,
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
