import { useState } from "react";
import { useNavigate } from "react-router-dom";

import { AuthLayout } from "@/features/auth/AuthLayout";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { useApiClient } from "@/core/api/apiClient";

export default function RegisterPage() {
  const api = useApiClient();
  const navigate = useNavigate();

  const [email, setEmail] = useState("");
  const [displayName, setDisplayName] = useState("");
  const [password, setPassword] = useState("");
  const [confirm, setConfirm] = useState("");

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const avatarLetter = displayName?.trim()?.[0]?.toUpperCase() ?? "?";

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError(null);

    if (password !== confirm) {
      setError("Les mots de passe ne correspondent pas.");
      return;
    }

    if (password.length < 8) {
      setError("Le mot de passe doit contenir 8 caractères.");
      return;
    }

    try {
      setLoading(true);

      await api.post("/api/v1/auth/register", {
        email,
        password,
        displayName
      });

      navigate("/confirm-email-sent");
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : "Erreur inconnue";

      if (msg.includes("Email already in use") || msg.includes("409")) {
        setError("Cet email est déjà utilisé.");
      } else {
        setError(msg);
      }
    } finally {
      setLoading(false);
    }
  }

  return (
    <AuthLayout title="Créer un compte">
      <div className="flex justify-center">
        <Avatar className="h-16 w-16 shadow-md">
          <AvatarFallback className="text-2xl font-bold bg-primary/10 text-primary">
            {avatarLetter}
          </AvatarFallback>
        </Avatar>
      </div>

      {error && (
        <div className="mb-4 text-red-600 text-sm text-center">{error}</div>
      )}

      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <Label htmlFor="email">Email</Label>
          <Input
            id="email"
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            className="mt-1"
          />
        </div>

        <div>
          <Label htmlFor="displayName">Nom affiché</Label>
          <Input
            id="displayName"
            value={displayName}
            onChange={(e) => setDisplayName(e.target.value)}
            required
            minLength={3}
            className="mt-1"
          />
        </div>

        <div>
          <Label htmlFor="password">Mot de passe</Label>
          <Input
            id="password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            className="mt-1"
          />
        </div>

        <div>
          <Label htmlFor="confirm">Confirmer le mot de passe</Label>
          <Input
            id="confirm"
            type="password"
            value={confirm}
            onChange={(e) => setConfirm(e.target.value)}
            required
            className="mt-1"
          />
        </div>

        <Button type="submit" className="w-full" disabled={loading}>
          {loading ? "Création..." : "Créer le compte"}
        </Button>

        <div className="text-center text-sm mt-2">
          <a href="/login" className="text-primary hover:underline">
            Déjà inscrit ? Se connecter
          </a>
        </div>
      </form>
    </AuthLayout>
  );
}
