import { apiClient } from "../api/apiClient";
import { API } from "../api/endpoints";
import type { LoginResponse } from "../models/Auth";

export const AuthService = {
  async login(email: string, password: string): Promise<LoginResponse> {
    const result = await apiClient.post<LoginResponse>(API.login, { email, password });

    localStorage.setItem("token", result.accessToken);
    localStorage.setItem("user", JSON.stringify(result.user));

    return {accessToken: result.accessToken,  refreshToken: result.refreshToken,  user: result.user};
  },

  logout() {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
  },

  getUser() {
    const raw = localStorage.getItem("user");
    return raw ? JSON.parse(raw) : null;
  },

  getToken() {
    return localStorage.getItem("token");
  }
};
