import "@testing-library/jest-dom";

// ------------------------------------------------------
// 1. Patch matchMedia (react-hot-toast / Tailwind dark mode)
// ------------------------------------------------------
Object.defineProperty(window, "matchMedia", {
  writable: true,
  value: (query: string) => ({
    matches: false,
    media: query,
    onchange: null,
    addEventListener: () => {},
    removeEventListener: () => {},
    addListener: () => {},
    removeListener: () => {},
    dispatchEvent: () => false
  })
});

// ------------------------------------------------------
// 2. MSW – serveur unique pour tous les tests
// ------------------------------------------------------
import { server } from "@/tests/msw-server";

// ------------------------------------------------------
// 3. Démarrage / arrêt MSW
// ------------------------------------------------------
beforeAll(() => server.listen({ onUnhandledRequest: "warn" }));
afterEach(() => server.resetHandlers());
afterAll(() => server.close());

class ResizeObserver {
  observe() {}
  unobserve() {}
  disconnect() {}
}

global.ResizeObserver = ResizeObserver;

declare global {
  interface Window {
    ResizeObserver: typeof ResizeObserver;
  }
}
