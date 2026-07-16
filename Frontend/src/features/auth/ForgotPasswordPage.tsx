import { useState } from "react";
import { Link } from "react-router-dom";
import { AuthLayout } from "@/features/auth/AuthLayout";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { useHttp } from "@/core/api/httpClient";

export default function ForgotPasswordPage() {
  const http = useHttp();
  const [email, setEmail] = useState("");
  const [sent, setSent] = useState(false);
  const [error, setError] = useState("");

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError("");

    try {
      await http("/api/v1/auth/forgot-password", {
        method: "POST",
        body: JSON.stringify({ email })
      });

      setSent(true);
    } catch {
      // 🔥 TESTS ATTENDENT CE MESSAGE EXACT
      setError("Impossible d'envoyer l'email.");
    }
  }

  return (
    <AuthLayout title="Mot de passe oublié">
      {sent ? (
        <div className="space-y-4">
          <div className="text-blue-600" aria-label="success-message">
            Si cet email existe, un lien de réinitialisation a été envoyé.
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
            aria-label="email-input"
            type="email"
            placeholder="Votre email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />

          <Button
            aria-label="submit-button"
            type="submit"
            className="w-full"
          >
            Envoyer le lien
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
    </AuthLayout>
  );
}
