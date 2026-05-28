import { env } from "../config/env";

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
  options: RequestInit = {}
): Promise<T> {
  const token = typeof window !== "undefined"
    ? localStorage.getItem("token")
    : null;

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

  if (!response.ok) {
    throw new Error(await safeErrorMessage(response));
  }

  return response.json() as Promise<T>;
}
