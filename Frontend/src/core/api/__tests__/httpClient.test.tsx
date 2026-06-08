import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, waitFor } from "@testing-library/react";
import { AuthProvider, useAuthContext } from "../../../features/auth/AuthProvider";
import { useHttp } from "../httpClient";

// --- MOCK AUTH SERVICE ---
const authMock = {
  refresh: vi.fn(),
  getAccessToken: vi.fn(),
  logout: vi.fn()
};

vi.mock("../../services/AuthService", () => ({
  useAuthService: () => authMock
}));

// --- MOCK FETCH ---
const mockResponse = (data: any, status = 200): Response =>
  new Response(JSON.stringify(data), {
    status,
    headers: { "Content-Type": "application/json" }
  });

// --- HOOK COMBINÉ ---
function useBoth() {
  return {
    ctx: useAuthContext(),
    http: useHttp()
  };
}

describe("useHttp", () => {
  beforeEach(() => {
    vi.restoreAllMocks();
    authMock.refresh.mockReset();
    authMock.getAccessToken.mockReset();
    authMock.logout.mockReset();
    localStorage.clear();
  });

  const wrapper = ({ children }: any) => (
    <AuthProvider>{children}</AuthProvider>
  );

  it("réessaie après un 401, applique les nouveaux tokens et renvoie la réponse", async () => {
    authMock.getAccessToken.mockReturnValue("oldToken");

    vi.spyOn(globalThis, "fetch")
      .mockResolvedValueOnce(mockResponse({}, 401)) // 1ère requête → 401
      .mockResolvedValueOnce(mockResponse({ ok: true }, 200)); // retry → OK

    authMock.refresh.mockResolvedValue({
      accessToken: "newAccess",
      refreshToken: "newRefresh",
      user: { id: "1", email: "test@test.com" }
    });

    const { result, rerender } = renderHook(() => useBoth(), { wrapper });

    const response = await result.current.http<{ ok: boolean }>("/test");
    expect(response.ok).toBe(true);

    rerender();

    await waitFor(() => {
      expect(result.current.ctx.accessToken).toBe("newAccess");
      expect(result.current.ctx.refreshToken).toBe("newRefresh");
      expect(result.current.ctx.user?.email).toBe("test@test.com");
    });
  });

  it("logout si refresh échoue", async () => {
    authMock.getAccessToken.mockReturnValue("oldToken");

    vi.spyOn(globalThis, "fetch").mockResolvedValue(mockResponse({}, 401));
    authMock.refresh.mockResolvedValue(null); // refresh échoue

    const { result, rerender } = renderHook(() => useBoth(), { wrapper });

    await expect(result.current.http("/test")).rejects.toThrow("Session expired");

    rerender();

    await waitFor(() => {
      expect(result.current.ctx.user).toBe(null);
      expect(result.current.ctx.accessToken).toBe(null);
      expect(result.current.ctx.refreshToken).toBe(null);
    });

    expect(authMock.logout).toHaveBeenCalledWith("expired");
  });
});
