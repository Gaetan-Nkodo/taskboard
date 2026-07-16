import { useSearchParams, Link } from "react-router-dom";
import { AuthLayout } from "@/features/auth/AuthLayout";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import toast from "react-hot-toast";
import { useState } from "react";

export default function ResetPasswordPage() {
  const [params] = useSearchParams();
  const token = params.get("token");

  const [password, setPassword] = useState("");
  const [done, setDone] = useState(false);
  const [error, setError] = useState("");

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError("");

    try {
      const response = await fetch(
        `${import.meta.env.VITE_API_URL}/api/v1/auth/reset-password`,
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ token, newPassword: password })
        }
      );

      if (!response.ok) {
        throw new Error(await response.text());
      }

      setDone(true);
    } catch (err) {
      const msg = err instanceof Error ? err.message : "Erreur inconnue";

      if (
        msg.includes("Invalid") ||
        msg.includes("401") ||
        msg.includes("400")
      ) {
        setError("Lien invalide ou expiré.");
      } else {
        setError(msg);
      }
    }
  }

  return (
    <AuthLayout title="Réinitialiser le mot de passe">
      {done ? (
        <div className="space-y-4">
          <div className="text-green-600" aria-label="success-message">
            Mot de passe mis à jour. Vous pouvez maintenant vous connecter.
          </div>

          <Link
            to="/login"
            aria-label="back-to-login"
            className="block text-center text-sm text-primary hover:underline"
          >
            Retour à la connexion
          </Link>
        </div>
      ) : (
        <form onSubmit={handleSubmit} className="space-y-4">
          {error && (
            <div className="text-red-600 text-sm" aria-label="error-message">
              {error}
            </div>
          )}

          <Input
            aria-label="password-input"
            type="password"
            placeholder="Nouveau mot de passe"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />

          <Button aria-label="submit-button" className="w-full">
            Mettre à jour
          </Button>

          <Link
            to="/login"
            aria-label="cancel-link"
            className="block text-center text-sm text-muted-foreground hover:text-primary"
          >
            Retour à la connexion
          </Link>
        </form>
      )}
    </AuthLayout>
  );
}
