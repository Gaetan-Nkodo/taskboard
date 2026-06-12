import type { ReactNode } from "react";
import { Toaster } from "react-hot-toast";
import { AuthProvider } from "../features/auth/AuthProvider";

export function TestProviders({ children }: { children: ReactNode }) {
  return (
    <AuthProvider>
      <Toaster />
      {children}
    </AuthProvider>
  );
}
