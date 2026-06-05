import { defineConfig } from "vitest/config";

export default defineConfig({
  test: {
    globals: true,
    environment: "jsdom",
    setupFiles: [
      "./src/setupTests.ts",
      "./src/tests/setup-msw.ts",
      "./src/tests/setup-integration.ts" // ✔️ uniquement ici
    ]
  }
});
