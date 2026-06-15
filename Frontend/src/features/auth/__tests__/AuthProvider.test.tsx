import { renderHook, act, waitFor } from "@testing-library/react";
import { AuthProvider, useAuthContext } from "../AuthProvider";

function wrapper({ children }: { children: React.ReactNode }) {
  return <AuthProvider>{children}</AuthProvider>;
}

describe("AuthProvider", () => {
  beforeEach(() => {
    localStorage.clear();
  });

  it("applyTokens met à jour le contexte ET le localStorage", async () => {
    const { result } = renderHook(() => useAuthContext(), { wrapper });

    act(() => {
      result.current.applyTokens({
        accessToken: "newAccess",
        refreshToken: "newRefresh",
        user: { id: "2", email: "new@test.com", displayName: "New User" }
      });
    });

    await waitFor(() => {
      expect(result.current.accessToken).toBe("newAccess");
      expect(result.current.refreshToken).toBe("newRefresh");
      expect(result.current.user?.email).toBe("new@test.com");
    });

    expect(localStorage.getItem("accessToken")).toBe("newAccess");
    expect(localStorage.getItem("refreshToken")).toBe("newRefresh");
    expect(JSON.parse(localStorage.getItem("user")!)).toEqual({
      id: "2",
      email: "new@test.com",
      displayName: "New User"
    });
  });

  it("logout nettoie le contexte et le localStorage", async () => {
    localStorage.setItem("user", JSON.stringify({ id: "1" }));
    localStorage.setItem("accessToken", "abc123");
    localStorage.setItem("refreshToken", "ref456");

    const { result } = renderHook(() => useAuthContext(), { wrapper });

    act(() => {
      result.current.logout();
    });

    await waitFor(() => {
      expect(result.current.user).toBe(null);
      expect(result.current.accessToken).toBe(null);
      expect(result.current.refreshToken).toBe(null);
    });

    expect(localStorage.getItem("user")).toBe(null);
    expect(localStorage.getItem("accessToken")).toBe(null);
    expect(localStorage.getItem("refreshToken")).toBe(null);
  });

  it("restaure la session depuis localStorage au montage", async () => {
    localStorage.setItem(
      "user",
      JSON.stringify({ id: "10", email: "restore@test.com", displayName: "Restored" })
    );
    localStorage.setItem("accessToken", "restoredAccess");
    localStorage.setItem("refreshToken", "restoredRefresh");

    const { result } = renderHook(() => useAuthContext(), { wrapper });

    await waitFor(() => {
      expect(result.current.user?.email).toBe("restore@test.com");
      expect(result.current.accessToken).toBe("restoredAccess");
      expect(result.current.refreshToken).toBe("restoredRefresh");
      expect(result.current.loading).toBe(false);
    });
  });
});
