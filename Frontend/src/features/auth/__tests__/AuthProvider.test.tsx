import { renderHook } from "@testing-library/react";
import { AuthProvider, useAuthContext } from "../AuthContext";
import { describe, it, expect, beforeEach } from "vitest";

describe("AuthProvider", () => {
  beforeEach(() => {
    localStorage.clear();
  });

  it("auto-login recharge l’utilisateur depuis localStorage", () => {
    localStorage.setItem("user", JSON.stringify({
      id: "1",
      email: "test@test.com",
      name: "Gaetan"
    }));

    const wrapper = ({ children }: any) => (
      <AuthProvider>{children}</AuthProvider>
    );

    const { result } = renderHook(() => useAuthContext(), { wrapper });

    expect(result.current.user?.email).toBe("test@test.com");
  });
});
