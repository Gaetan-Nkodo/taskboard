import { useAuthContext } from "../../features/auth/AuthProvider";
import { useAuthService } from "../services/AuthService";

export function useHttp() {
  const { applyTokens, logout } = useAuthContext();
  const auth = useAuthService();

  return async function http<T>(
    url: string,
    options: RequestInit = {},
    retry = true
  ): Promise<T> {
    const token = auth.getAccessToken();

    const headers: HeadersInit = {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...(options.headers || {})
    };

    const response = await fetch(url, { ...options, headers });

    if (response.status === 401 && retry) {
      const refreshed = await auth.refresh();

      if (refreshed) {
        applyTokens(refreshed);
        return http<T>(url, options, false);
      }
      
      auth.logout();
      logout();
      throw new Error("Session expired");
    }

    if (!response.ok) {
      throw new Error(await response.text());
    }

    return response.json();
  };
}
