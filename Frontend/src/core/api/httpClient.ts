import { env } from "../config/env";

export const http = async <T>(url: string, options: RequestInit = {}): Promise<T> => {
  const token = localStorage.getItem("token");

  const response = await fetch(`${env.apiUrl}${url}`, {
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...options.headers
    },
    ...options
  });

  if (!response.ok) {
    throw new Error(`HTTP error ${response.status}`);
  }

  return response.json() as Promise<T>;
};
