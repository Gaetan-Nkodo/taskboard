import { describe, it, expect, beforeEach } from "vitest";
import { useAuth } from "../useAuth";

describe("logout", () => {
  beforeEach(() => {
    localStorage.clear();
  });

  it("supprime token et user", () => {
    // Arrange
    localStorage.setItem("token", "abc");
    localStorage.setItem("user", JSON.stringify({ id: "1" }));

    const { logout, getUser } = useAuth();

    // Act
    logout();

    // Assert
    expect(localStorage.getItem("token")).toBeNull();
    expect(getUser()).toBeNull();
  });
});
