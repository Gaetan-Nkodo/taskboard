import "@testing-library/jest-dom";
import { beforeAll, afterAll, afterEach, beforeEach } from "vitest";
import { setupServer } from "msw/node";
import { handlers } from "./tests-integration/msw/handlers";

export const server = setupServer(...handlers);
beforeEach(() => {
  localStorage.clear();
});

beforeAll(() => server.listen({ onUnhandledRequest: "error" }));
afterEach(() => server.resetHandlers());
afterAll(() => server.close());
