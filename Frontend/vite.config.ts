import { defineConfig } from "vitest/config";
import react from "@vitejs/plugin-react";
import path from "path";

export default defineConfig({
  plugins: [react()],

  resolve: {
    alias: {
      "@": path.resolve(__dirname, "src")
    }
  },

  build: {
    cssMinify: false,
    target: "esnext"
  },

  // Vitest intégré dans le même fichier
  test: {
    globals: true,
    environment: "jsdom",
    css: false,
    setupFiles: [
      "./src/setupTests.ts",
      "./src/tests/setup-msw.ts"
    ]
  }
});
