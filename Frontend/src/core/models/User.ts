export interface User {
  id: string;
  email: string;
  displayName: string;

  role?: "user" | "admin" | "owner";
  avatarUrl?: string;

  createdAt?: string;
  updatedAt?: string;

  // Pour audit / sécurité
  lastLoginAt?: string;
  lastIp?: string;

  // Pour multi‑tenant
  organizationId?: string;
}
