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

  css: {
    transformer: "postcss"
  },

  build: {
    cssMinify: false,
    target: "esnext"
  },

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
