import { http } from "./httpClient";

export const apiClient = {
  get: <T>(url: string, params?: Record<string, any>) => {
    const query = params
      ? "?" + new URLSearchParams(params as any).toString()
      : "";
    return http<T>(url + query, { method: "GET" });
  },

  post: <T>(url: string, body?: any) => {
    return http<T>(url, {
      method: "POST",
      body
    });
  },

  put: <T>(url: string, body?: any) => {
    return http<T>(url, {
      method: "PUT",
      body
    });
  },

  patch: <T>(url: string, body?: any) => {
    return http<T>(url, {
      method: "PATCH",
      body
    });
  },

  delete: <T>(url: string) => {
    return http<T>(url, { method: "DELETE" });
  }
};
