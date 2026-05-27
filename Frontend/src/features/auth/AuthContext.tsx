import { createContext, useContext, useState, useEffect } from "react";
import type { LoginResponse } from "../../core/models/Auth";
import { useAuth } from "./useAuth";

interface AuthContextValue {
  user: LoginResponse["user"] | null;
  login: (email: string, password: string) => Promise<LoginResponse["user"]>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const { login: loginApi, logout: logoutApi, getUser } = useAuth();
  const [user, setUser] = useState(getUser());

  useEffect(() => {
    const savedUser = getUser();
    if (savedUser) {
      setUser(savedUser);
    }
  }, []);

  const login = async (email: string, password: string) => {
    const result = await loginApi({ email, password });
    setUser(result);
    return result; // navigation dans LoginPage
  };

  const logout = () => {
    logoutApi();
    setUser(null); // navigation dans Header
  };

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuthContext = () => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("AuthContext missing");
  return ctx;
};
