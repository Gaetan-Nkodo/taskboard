import { API } from "../api/endpoints";
import type { LoginResponse } from "../models/Auth";

const ACCESS_TOKEN_KEY = "accessToken";
const REFRESH_TOKEN_KEY = "refreshToken";
const USER_KEY = "user";

export function useAuthService() {
  async function login(email: string, password: string): Promise<LoginResponse> {
    const response = await fetch(API.login, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email, password })
    });

    const result = await response.json();

    localStorage.setItem(ACCESS_TOKEN_KEY, result.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, result.refreshToken);
    localStorage.setItem(USER_KEY, JSON.stringify(result.user));

    return result;
  }

  async function refresh(): Promise<LoginResponse | null> {
    const refreshToken = localStorage.getItem(REFRESH_TOKEN_KEY);
    if (!refreshToken) return null;

    try {
      const response = await fetch(API.refresh, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ refreshToken })
      });

      const result = await response.json();

      localStorage.setItem(ACCESS_TOKEN_KEY, result.accessToken);
      localStorage.setItem(REFRESH_TOKEN_KEY, result.refreshToken);
      localStorage.setItem(USER_KEY, JSON.stringify(result.user));

      return result;
    } catch {
      logout();
      return null;
    }
  }

  function logout(reason?: string) {
    localStorage.clear();
    if (reason) localStorage.setItem("logoutReason", reason);
    window.location.href = "/login";
  }

  async function changePassword(currentPassword: string, newPassword: string) {
    const accessToken = localStorage.getItem(ACCESS_TOKEN_KEY);

    const response = await fetch(API.changePassword, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + accessToken
      },
      body: JSON.stringify({ currentPassword, newPassword })
    });

    if (response.status === 401) {
      logout("Votre session a expiré.");
      return null;
    }
    return response;
  }

  return {
    login,
    refresh,
    logout,
    changePassword,
    getUser: () => JSON.parse(localStorage.getItem(USER_KEY) || "null"),
    getAccessToken: () => localStorage.getItem(ACCESS_TOKEN_KEY),
    getRefreshToken: () => localStorage.getItem(REFRESH_TOKEN_KEY)
  };
}
