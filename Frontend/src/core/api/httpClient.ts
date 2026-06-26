import { useAuthContext } from "@/features/auth/AuthProvider";

const API_URL = import.meta.env.VITE_API_URL;

// Normalise les URLs : accepte "/api/..." ou "http://..."
function resolveUrl(url: string): string {
  if (url.startsWith("http://") || url.startsWith("https://")) {
    return url;
  }
  return `${API_URL}${url.startsWith("/") ? url : `/${url}`}`;
}

export function useHttp() {
  const { accessToken, refreshToken, applyTokens, logout } = useAuthContext();

  async function http<T>(url: string, options: RequestInit = {}, retry = true): Promise<T> {
    const finalUrl = resolveUrl(url);

    const headers: HeadersInit = {
      ...(options.method !== "GET" ? { "Content-Type": "application/json" } : {}),
      ...(accessToken ? { Authorization: `Bearer ${accessToken}` } : {}),
      ...(options.headers || {})
    };

    const response = await fetch(finalUrl, { ...options, headers });

    // Gestion du refresh token
    if (response.status === 401 && retry && accessToken && refreshToken) {
      try {
        const refreshRes = await fetch(resolveUrl("/api/v1/auth/refresh"), {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ refreshToken })
        });

        if (refreshRes.ok) {
          const refreshed = await refreshRes.json();
          applyTokens(refreshed);
          return http<T>(url, options, false);
        }
      } catch {
        // ignore
      }

      logout();
      throw new Error("Session expired");
    }

    // Gestion des erreurs backend
    if (!response.ok) {
      let message = "Unknown error";

      try {
        const text = await response.text();
        if (text) {
          try {
            const json = JSON.parse(text);
            message = json.error || json.message || text;
          } catch {
            message = text;
          }
        }
      } catch {
        // ignore
      }

      throw new Error(message);
    }

    // 204 No Content
    if (response.status === 204) {
      return null as T;
    }

    // JSON ou texte
    const raw = await response.text();
    if (!raw) return null as T;

    try {
      return JSON.parse(raw) as T;
    } catch {
      return raw as unknown as T;
    }
  }

  return http;
}
