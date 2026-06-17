import { describe, it, expect, beforeEach } from "vitest";
import { renderHook, act } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { AuthProvider, useAuthContext } from "../AuthProvider";

function wrapper({ children }: { children: React.ReactNode }) {
  return (
    <MemoryRouter>
      <AuthProvider>{children}</AuthProvider>
    </MemoryRouter>
  );
}

describe("logout", () => {
  beforeEach(() => {
    localStorage.clear();
  });

  it("supprime token et user", () => {
    localStorage.setItem("accessToken", "abc");
    localStorage.setItem("refreshToken", "ref");
    localStorage.setItem("user", JSON.stringify({ id: "1" }));

    const { result } = renderHook(() => useAuthContext(), { wrapper });

    act(() => {
      result.current.logout();
    });

    expect(localStorage.getItem("accessToken")).toBeNull();
    expect(localStorage.getItem("user")).toBeNull();
  });
});
