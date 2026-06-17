import { useAuthContext } from "@/features/auth/AuthProvider";

const API_URL = import.meta.env.VITE_API_URL;

export function useHttp() {
  const { accessToken, refreshToken, applyTokens, logout } = useAuthContext();

  return async function http<T>(url: string, options: RequestInit = {}, retry = true): Promise<T> {
    const headers: HeadersInit = {
      "Content-Type": "application/json",
      ...(accessToken ? { Authorization: `Bearer ${accessToken}` } : {}),
      ...(options.headers || {})
    };

    const response = await fetch(`${API_URL}${url}`, { ...options, headers });

    if (response.status === 401 && retry && accessToken && refreshToken) {
      try {
        const refreshResponse = await fetch(`${API_URL}/api/v1/auth/refresh`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ refreshToken })
        });

        if (refreshResponse.ok) {
          const refreshed = await refreshResponse.json();
          applyTokens(refreshed);
          return http<T>(url, options, false);
        }
      } catch {
        // ignore refresh errors
      }

      logout();
      throw new Error("Session expired");
    }

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(errorText || "Unknown error");
    }

    const text = await response.text();
    if (!text) return null as T;

    return JSON.parse(text) as T;
  };
}
