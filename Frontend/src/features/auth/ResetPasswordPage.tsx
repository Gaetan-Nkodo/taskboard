import { useState } from "react";
import { useSearchParams, Link } from "react-router-dom";
import { Card, Input, Button } from "@/components/ui";
import { useHttp } from "@/core/api/httpClient";

export default function ResetPasswordPage() {
  const http = useHttp();
  const [params] = useSearchParams();
  const token = params.get("token");

  const [password, setPassword] = useState("");
  const [done, setDone] = useState(false);
  const [error, setError] = useState("");

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError("");

    try {
      await http("/api/v1/auth/reset-password", {
        method: "POST",
        body: JSON.stringify({ token, newPassword: password })
      });

      setDone(true);

    } catch (err) {
      const message =
        err instanceof Error ? err.message : "Erreur inconnue";

      if (
        message.includes("Invalid") ||
        message.includes("401") ||
        message.includes("400")
      ) {
        setError("Lien invalide ou expiré.");
      } else {
        setError(message);
      }
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center px-4 sm:px-6 lg:px-8">
      <Card className="w-full max-w-sm p-6 animate-fadeIn">
        <h1 className="text-xl font-bold mb-4">Réinitialiser le mot de passe</h1>

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
              onChange={e => setPassword(e.target.value)}
              required
            />

            <Button
              aria-label="submit-button"
              type="submit"
              className="w-full"
            >
              Mettre à jour
            </Button>

            <Link
              to="/login"
              aria-label="cancel-link"
              className="block text-center text-sm text-muted-foreground hover:text-primary transition-colors"
            >
              Retour à la connexion
            </Link>
          </form>
        )}
      </Card>
    </div>
  );
}
