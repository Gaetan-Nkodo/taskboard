import { defineConfig } from "vitest/config";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  test: {
    globals: true,
    environment: "jsdom",
    env: {
      VITE_API_URL: "http://localhost"
    },
    setupFiles: ["./src/setupTests.ts"],
    css: false,
    coverage: {
      reporter: ["text", "html"]
    }
  }
});
