import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, act } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { useAuth } from "../useAuth";

function wrapper({ children }: { children: React.ReactNode }) {
  return <MemoryRouter>{children}</MemoryRouter>;
}

describe("useAuth", () => {
  beforeEach(() => {
    localStorage.clear();
    vi.restoreAllMocks();
  });

  it("login stocke token et user", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      json: async () => ({
        accessToken: "abc",
        refreshToken: "xyz",
        user: { email: "test@test.com" }
      })
    } as any);

    const { result } = renderHook(() => useAuth(), { wrapper });

    await act(async () => {
      await result.current.login({ email: "test@test.com", password: "123" });
    });

    const user = result.current.getUser();
    expect(user?.email).toBe("test@test.com");
    expect(localStorage.getItem("token")).toBe("abc");
  });

  it("logout supprime token et user", () => {
    localStorage.setItem("token", "abc");
    localStorage.setItem("user", JSON.stringify({ id: "1" }));

    const { result } = renderHook(() => useAuth(), { wrapper });

    act(() => {
      result.current.logout();
    });

    expect(localStorage.getItem("token")).toBeNull();
    expect(result.current.getUser()).toBeNull();
  });

  it("login renvoie une erreur si backend renvoie une erreur", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: false,
      text: async () => "Invalid credentials"
    } as any);

    const { result } = renderHook(() => useAuth(), { wrapper });

    await expect(
      result.current.login({ email: "bad", password: "wrong" })
    ).rejects.toThrow("Invalid credentials");
  });
});
