import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, act, waitFor } from "@testing-library/react";
import { AuthProvider, useAuthContext } from "@/features/auth/AuthProvider";
import { useHttp } from "../httpClient";
import { API } from "@/core/api/endpoints";

// -------------------------------------------------------------
// 🔥 Patch JSDOM pour empêcher toute navigation réelle
// -------------------------------------------------------------
delete (window as any).location;
(window as any).location = { href: "", assign: vi.fn() };

// -------------------------------------------------------------
// 🔥 Mock AuthService — IMPORTANT : chemin ABSOLU réel
// -------------------------------------------------------------
const authMock = {
  refresh: vi.fn(),
  getAccessToken: vi.fn(),
  logout: vi.fn(),
};

vi.mock("@/core/services/AuthService", () => ({
  useAuthService: () => authMock,
}));

// -------------------------------------------------------------
// 🔥 Helper Response
// -------------------------------------------------------------
const mockResponse = (data: any, status = 200): Response =>
  new Response(JSON.stringify(data), {
    status,
    headers: { "Content-Type": "application/json" },
  });

// Hook combiné
function useBoth() {
  return {
    ctx: useAuthContext(),
    http: useHttp(),
  };
}

describe("useHttp", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    localStorage.clear();
  });

  const wrapper = ({ children }: any) => (
    <AuthProvider>{children}</AuthProvider>
  );

  // -------------------------------------------------------------
  // 🔥 TEST 1 — RETRY APRÈS 401
  // -------------------------------------------------------------
  it("réessaie après un 401, applique les nouveaux tokens et renvoie la réponse", async () => {
    authMock.getAccessToken.mockReturnValue("oldToken");

    global.fetch = vi.fn((input: RequestInfo | URL) => {
      const url = typeof input === "string" ? input : input.toString();

      if (url === "/test") {
        // 1ère requête → 401
        if (!authMock.refresh.mock.calls.length)
          return Promise.resolve(mockResponse({}, 401));

        // retry → 200
        return Promise.resolve(mockResponse({ ok: true }, 200));
      }

      if (url === API.refresh) {
        return Promise.resolve(
          mockResponse({
            accessToken: "newAccess",
            refreshToken: "newRefresh",
            user: { id: "1", email: "test@test.com" },
          })
        );
      }

      return Promise.reject(new Error("URL non mockée : " + url));
    }) as unknown as typeof fetch;

    authMock.refresh.mockResolvedValue({
      accessToken: "newAccess",
      refreshToken: "newRefresh",
      user: { id: "1", email: "test@test.com" },
    });

    const { result } = renderHook(() => useBoth(), { wrapper });

    // 🔥 Fix TS2339
    let response!: { ok: boolean };

    await act(async () => {
      response = await result.current.http("/test");
    });

    expect(response.ok).toBe(true);

    await waitFor(() => {
      expect(result.current.ctx.accessToken).toBe("newAccess");
      expect(result.current.ctx.refreshToken).toBe("newRefresh");
    });
  });

  // -------------------------------------------------------------
  // 🔥 TEST 2 — REFRESH ÉCHOUE → LOGOUT
  // -------------------------------------------------------------
  it("logout si refresh échoue", async () => {
    authMock.getAccessToken.mockReturnValue("oldToken");

    global.fetch = vi.fn((input: RequestInfo | URL) => {
      const url = typeof input === "string" ? input : input.toString();

      if (url === "/test") {
        return Promise.resolve(mockResponse({}, 401));
      }

      if (url === API.refresh) {
        return Promise.resolve(mockResponse({}, 500));
      }

      return Promise.reject(new Error("URL non mockée : " + url));
    }) as unknown as typeof fetch;

    authMock.refresh.mockResolvedValue(null);

    const { result } = renderHook(() => useBoth(), { wrapper });

    await act(async () => {
      await expect(result.current.http("/test")).rejects.toThrow("Session expired");
    });

    expect(authMock.logout).toHaveBeenCalledWith("expired");
  });
});
