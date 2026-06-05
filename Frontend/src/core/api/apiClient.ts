import { useHttp } from "./httpClient";

export function useApiClient() {
  const http = useHttp();

  return {
    get: <T>(url: string, params?: Record<string, any>) => {
      const query = params
        ? "?" + new URLSearchParams(params as any).toString()
        : "";
      return http<T>(url + query, { method: "GET" });
    },

    post: <T>(url: string, body?: any) => {
      return http<T>(url, {
        method: "POST",
        body: JSON.stringify(body)
      });
    },

    put: <T>(url: string, body?: any) => {
      return http<T>(url, {
        method: "PUT",
        body: JSON.stringify(body)
      });
    },

    patch: <T>(url: string, body?: any) => {
      return http<T>(url, {
        method: "PATCH",
        body: JSON.stringify(body)
      });
    },

    delete: <T>(url: string) => {
      return http<T>(url, { method: "DELETE" });
    }
  };
}
