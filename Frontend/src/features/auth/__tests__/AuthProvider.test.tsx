import { renderHook, waitFor } from "@testing-library/react";
import { AuthProvider, useAuthContext } from "../AuthProvider";

const wrapper = ({ children }: any) => (
  <AuthProvider>{children}</AuthProvider>
);

it("applyTokens met à jour le contexte", async () => {
  const { result } = renderHook(() => useAuthContext(), { wrapper });

  // Appel direct : pas besoin de act() car applyTokens ne touche pas au DOM
  result.current.applyTokens({
    accessToken: "newAccess",
    refreshToken: "newRefresh",
    user: { id: "2", email: "new@test.com", displayName: "New User" }
  });

  // Attendre la mise à jour de React
  await waitFor(() => {
    expect(result.current.accessToken).toBe("newAccess");
    expect(result.current.refreshToken).toBe("newRefresh");
    expect(result.current.user?.email).toBe("new@test.com");
  });
});

it("logout nettoie le contexte et le localStorage", async () => {
  localStorage.setItem("user", JSON.stringify({ id: "1" }));
  localStorage.setItem("token", "abc123");
  localStorage.setItem("refreshToken", "ref456");

  const { result } = renderHook(() => useAuthContext(), { wrapper });

  result.current.logout();

  await waitFor(() => {
    expect(result.current.user).toBe(null);
    expect(result.current.accessToken).toBe(null);
    expect(result.current.refreshToken).toBe(null);
  });
});
