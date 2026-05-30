import { renderHook, waitFor } from "@testing-library/react";
import { AuthProvider, useAuthContext } from "../AuthProvider";
import { describe, it, expect, beforeEach } from "vitest";

describe("AuthProvider", () => {
  beforeEach(() => {
    localStorage.clear();
  });

  it("auto-login recharge l’utilisateur depuis localStorage", async () => {
    localStorage.setItem("user", JSON.stringify({
      id: "1",
      email: "test@test.com",
      name: "Gaetan"
    }));
    localStorage.setItem("token", "abc123");
    localStorage.setItem("refreshToken", "ref456");

    const wrapper = ({ children }: any) => (
      <AuthProvider>{children}</AuthProvider>
    );

    const { result } = renderHook(() => useAuthContext(), { wrapper });

    // attendre la fin du loading
    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.user?.email).toBe("test@test.com");
    expect(result.current.accessToken).toBe("abc123");
    expect(result.current.refreshToken).toBe("ref456");
  });
});
