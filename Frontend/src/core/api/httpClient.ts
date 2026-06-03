import { env } from "../config/env";
import { AuthService } from "../services/AuthService";

async function safeErrorMessage(response: Response): Promise<string> {
  try {
    const text = await response.text();
    return text || `HTTP error ${response.status}`;
  } catch {
    return `HTTP error ${response.status}`;
  }
}

export async function http<T>(
  url: string,
  options: RequestInit = {},
  retry = true
): Promise<T> {
  const token = localStorage.getItem("token");

  const headers: HeadersInit = {
    "Content-Type": "application/json",
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
    ...(options.headers || {})
  };

  let body = options.body;
  if (body && typeof body === "object" && !(body instanceof FormData)) {
    body = JSON.stringify(body);
  }

  const response = await fetch(`${env.apiUrl}${url}`, {
    ...options,
    headers,
    body
  });

  // --- Auto refresh ---
  if (response.status === 401 && retry) {
    const refreshed = await AuthService.refresh();
    if (refreshed) {
      return http<T>(url, options, false); // retry une seule fois
    }
    AuthService.logout();
    throw new Error("Session expired");
  }

  if (!response.ok) {
    throw new Error(await safeErrorMessage(response));
  }

  return response.json() as Promise<T>;
}
