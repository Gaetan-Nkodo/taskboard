import { createContext, useContext, useEffect, useState } from "react";
import type { User } from "../../core/models/User";

interface AuthContextValue {
  user: User | null;
  accessToken: string | null;
  refreshToken: string | null;
  loading: boolean;

  setUser: (u: User | null) => void;
  setAccessToken: (t: string | null) => void;
  setRefreshToken: (t: string | null) => void;

  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [accessToken, setAccessToken] = useState<string | null>(null);
  const [refreshToken, setRefreshToken] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  // 🔄 Restauration session
  useEffect(() => {
    const storedUser = localStorage.getItem("user");
    const storedAccess = localStorage.getItem("token");
    const storedRefresh = localStorage.getItem("refreshToken");

    if (storedUser && storedAccess) {
      setUser(JSON.parse(storedUser));
      setAccessToken(storedAccess);
      setRefreshToken(storedRefresh);
    }

    setLoading(false);
  }, []);

  const logout = () => {
    localStorage.removeItem("user");
    localStorage.removeItem("token");
    localStorage.removeItem("refreshToken");

    setUser(null);
    setAccessToken(null);
    setRefreshToken(null);
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
        logout
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
