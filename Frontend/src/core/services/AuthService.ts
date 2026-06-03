import { apiClient } from "../api/apiClient";
import { API } from "../api/endpoints";
import type { LoginResponse } from "../models/Auth";

const ACCESS_TOKEN_KEY = "accessToken";
const REFRESH_TOKEN_KEY = "refreshToken";
const USER_KEY = "user";

export const AuthService = {
  async login(email: string, password: string): Promise<LoginResponse> {
    const result = await apiClient.post<LoginResponse>(API.login, { email, password });

    localStorage.setItem(ACCESS_TOKEN_KEY, result.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, result.refreshToken);
    localStorage.setItem(USER_KEY, JSON.stringify(result.user));

    return result;
  },

  async refresh(): Promise<LoginResponse | null> {
    const refreshToken = localStorage.getItem(REFRESH_TOKEN_KEY);
    if (!refreshToken) return null;

    try {
      const result = await apiClient.post<LoginResponse>(API.refresh, { refreshToken });

      localStorage.setItem(ACCESS_TOKEN_KEY, result.accessToken);
      localStorage.setItem(REFRESH_TOKEN_KEY, result.refreshToken);
      localStorage.setItem(USER_KEY, JSON.stringify(result.user));

      return result;
    } catch {
      this.logout();
      return null;
    }
  },

  logout() {
    // Optionnel : appeler l’API, mais on ne bloque pas sur l’erreur
    apiClient.post(API.logout, {}).catch(() => {});
    localStorage.clear();

    window.location.href = "/login";
  },

  getUser() {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? JSON.parse(raw) : null;
  },

  getAccessToken() {
    return localStorage.getItem(ACCESS_TOKEN_KEY);
  },

  getRefreshToken() {
    return localStorage.getItem(REFRESH_TOKEN_KEY);
  }
};
