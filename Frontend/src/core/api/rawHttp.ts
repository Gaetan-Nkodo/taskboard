const API_URL = import.meta.env.VITE_API_URL;

// Normalise les URLs : accepte "/api/..." ou "http://..."
function resolveUrl(url: string): string {
  if (url.startsWith("http://") || url.startsWith("https://")) {
    return url;
  }
  return `${API_URL}${url.startsWith("/") ? url : `/${url}`}`;
}

export async function rawHttp<T>(url: string, options: RequestInit = {}): Promise<T> {
  const finalUrl = resolveUrl(url);

  const response = await fetch(finalUrl, {
    ...options,
    headers: {
      ...(options.method !== "GET" ? { "Content-Type": "application/json" } : {}),
      ...(options.headers || {})
    }
  });

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
