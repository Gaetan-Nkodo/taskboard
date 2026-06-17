import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, act } from "@testing-library/react";
import { AuthProvider, useAuthContext } from "@/features/auth/AuthProvider";
import { useHttp } from "../httpClient";

function useBoth() {
  return {
    ctx: useAuthContext(),
    http: useHttp(),
  };
}

describe("useHttp", () => {
  beforeEach(() => {
    localStorage.setItem("accessToken", "oldToken");
    localStorage.setItem("refreshToken", "oldRefresh");
    localStorage.setItem("user", JSON.stringify({ id: "1" }));
  });

  const wrapper = ({ children }: any) => (
    <AuthProvider>{children}</AuthProvider>
  );

  it("réessaie après un 401 et applique les nouveaux tokens", async () => {
    global.fetch = vi.fn((input) => {
      const url = input.toString();

      if (url === `${import.meta.env.VITE_API_URL}/test`) {
        if ((global.fetch as any).mock.calls.length === 1)
          return Promise.resolve(new Response("{}", { status: 401 }));

        return Promise.resolve(
          new Response(JSON.stringify({ ok: true }), { status: 200 })
        );
      }

      if (url === `${import.meta.env.VITE_API_URL}/api/v1/auth/refresh`) {
        return Promise.resolve(
          new Response(
            JSON.stringify({
              accessToken: "newAccess",
              refreshToken: "newRefresh",
              user: { id: "1" },
            }),
            { status: 200 }
          )
        );
      }

      throw new Error("URL non mockée: " + url);
    });

    const { result } = renderHook(() => useBoth(), { wrapper });

    let response: any;

    await act(async () => {
      response = await result.current.http("/test");
    });

    expect(response.ok).toBe(true);
    expect(result.current.ctx.accessToken).toBe("newAccess");
  });

it("logout si refresh échoue", async () => {
  global.fetch = vi.fn((input) => {
    const url = input.toString();

    if (url === `${import.meta.env.VITE_API_URL}/test`) {
      return Promise.resolve(new Response("{}", { status: 401 }));
    }

    if (url === `${import.meta.env.VITE_API_URL}/api/v1/auth/refresh`) {
      return Promise.resolve(new Response("{}", { status: 500 }));
    }

    throw new Error("URL non mockée: " + url);
  });

  const { result } = renderHook(() => useBoth(), { wrapper });

  await act(async () => {
    await expect(result.current.http("/test")).rejects.toThrow("Session expired");
  });
});
});
