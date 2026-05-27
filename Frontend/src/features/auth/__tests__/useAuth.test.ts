import { describe, it, expect, vi, beforeEach } from "vitest";
import { useAuth } from "../useAuth";

describe("useAuth", () => {
  beforeEach(() => {
    localStorage.clear();
    vi.restoreAllMocks();
  });

  it("login stocke token et user", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      json: async () => ({
        token: "abc",
        user: { id: "1", email: "test@test.com", name: "Gaetan" }
      })
    } as any);

    const { login, getUser } = useAuth();

    await login({ email: "test@test.com", password: "123" });

    const user = getUser();
    expect(user.email).toBe("test@test.com");
    expect(localStorage.getItem("token")).toBe("abc");
  });

  it("logout supprime token et user", () => {
    localStorage.setItem("token", "abc");
    localStorage.setItem("user", JSON.stringify({ id: "1" }));

    const { logout, getUser } = useAuth();
    logout();

    expect(localStorage.getItem("token")).toBeNull();
    expect(getUser()).toBeNull();
  });

  it("login renvoie une erreur si backend renvoie une erreur", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: false,
      text: async () => "Invalid credentials"
    } as any);

    const { login } = useAuth();

    await expect(
      login({ email: "bad", password: "wrong" })
    ).rejects.toThrow("Invalid credentials");
  });
});
