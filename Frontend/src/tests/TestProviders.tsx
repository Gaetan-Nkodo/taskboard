import { AuthProvider } from "@/features/auth/AuthProvider";

export function TestProviders({ children }: { children: React.ReactNode }) {
  return <AuthProvider>{children}</AuthProvider>;
}
