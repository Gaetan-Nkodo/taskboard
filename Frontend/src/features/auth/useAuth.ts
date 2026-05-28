import { env } from "../../core/config/env";
import type { LoginRequest, LoginResponse } from "../../core/models/Auth";

export const useAuth = () => {
  const login = async (data: LoginRequest) => {
    const response = await fetch(`${env.apiUrl}/auth/login`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data)
    });

    if (!response.ok) {
      let message = "Erreur de connexion";
      try {
        const errorText = await response.text();
        if (errorText) message = errorText;
      } catch {}
      throw new Error(message);
    }

    const result = (await response.json()) as LoginResponse;

    localStorage.setItem("token", result.token);
    localStorage.setItem("user", JSON.stringify(result.user));

    return result;
  };

  const logout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
  };

  const getUser = () => {
    const raw = localStorage.getItem("user");
    return raw ? JSON.parse(raw) : null;
  };

  const getToken = () => localStorage.getItem("token");

  return { login, logout, getUser, getToken };
};
