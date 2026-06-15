import { useAuthContext } from "@/features/auth/AuthProvider";
import { useAuthService } from "../services/AuthService";

export function useHttp() {
  const { accessToken, applyTokens, logout: contextLogout } = useAuthContext();
  const auth = useAuthService();

  return async function http<T>(
    url: string,
    options: RequestInit = {},
    retry = true
  ): Promise<T> {
    const token = accessToken ?? auth.getAccessToken();

    const headers: HeadersInit = {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...(options.headers || {})
    };

    const response = await fetch(url, { ...options, headers });

    // -----------------------------
    // 🔥 401 → tentative de refresh
    // -----------------------------
    if (response.status === 401 && retry) {
      const refreshed = await auth.refresh();

      if (refreshed) {
        applyTokens(refreshed);
        return http<T>(url, options, false);
      }

      // ❗ IMPORTANT : un seul logout → AuthService
      auth.logout("expired");

      throw new Error("Session expired");
    }

    // -----------------------------
    // 🔥 autres erreurs HTTP
    // -----------------------------
    if (!response.ok) {
      throw new Error(await response.text());
    }

    return response.json();
  };
}
