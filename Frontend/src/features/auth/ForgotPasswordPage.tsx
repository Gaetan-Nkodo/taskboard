import { useState } from "react";
import { Link } from "react-router-dom";
import { Card, Input, Button } from "@/components/ui";
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

    } catch (err) {
      const message =
        err instanceof Error ? err.message : "Erreur inconnue";

      // Tests attendent un message générique
      setError("Impossible d'envoyer l'email.");
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center px-4 sm:px-6 lg:px-8">
      <Card className="w-full max-w-sm p-6 animate-fadeIn">
        <h1 className="text-xl font-bold mb-4">Mot de passe oublié</h1>

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
              onChange={e => setEmail(e.target.value)}
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
      </Card>
    </div>
  );
}
