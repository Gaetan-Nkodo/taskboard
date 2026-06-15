import { ReactNode } from "react";
import { MemoryRouter } from "react-router-dom";
import { ThemeProvider } from "@/components/theme/ThemeProvider";
import { AuthProvider } from "@/features/auth/AuthProvider";
import { Toaster } from "react-hot-toast";
import { render } from "@testing-library/react";

/**
 * Providers globaux utilisés dans 95% des tests :
 * - ThemeProvider
 * - AuthProvider
 * - Toaster
 */
export function AppProviders({ children }: { children: ReactNode }) {
  return (
    <ThemeProvider>
      <AuthProvider>
        <Toaster />
        {children}
      </AuthProvider>
    </ThemeProvider>
  );
}

/**
 * Version utilisée pour les tests de pages et composants
 * nécessitant un Router (MemoryRouter).
 */
export function renderWithRouter(
  ui: React.ReactElement,
  { route = "/" } = {}
) {
  window.history.pushState({}, "Test page", route);

  return render(
    <MemoryRouter initialEntries={[route]}>
      <AppProviders>{ui}</AppProviders>
    </MemoryRouter>
  );
}

/**
 * Version utilisée dans certains tests unitaires
 * (comme ChangePasswordPage.test.tsx)
 * où on veut juste les providers globaux,
 * sans router automatique.
 */
export function TestProviders({ children }: { children: ReactNode }) {
  return <AppProviders>{children}</AppProviders>;
}
