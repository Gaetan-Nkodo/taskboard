import { useState } from "react";
import { Link } from "react-router-dom";
import { Card, Input, Button } from "@/components/ui";

export default function ForgotPasswordPage() {
  const [email, setEmail] = useState("");
  const [sent, setSent] = useState(false);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();

    await fetch("/api/v1/auth/forgot-password", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email })
    });

    setSent(true);
  }

  return (
    <div className="min-h-screen flex items-center justify-center px-4 sm:px-6 lg:px-8">
      <Card className="w-full max-w-sm p-6 animate-fadeIn">
        <h1 className="text-xl font-bold mb-4">Mot de passe oublié</h1>

        {sent ? (
          <div className="space-y-4">
            <div className="text-blue-600">
              Si cet email existe, un lien de réinitialisation a été envoyé.
            </div>

            <Link
              to="/login"
              className="block text-center text-sm text-primary hover:underline"
            >
              Retour à la connexion
            </Link>
          </div>
        ) : (
          <form onSubmit={handleSubmit} className="space-y-4">
            <Input
              type="email"
              placeholder="Votre email"
              value={email}
              onChange={e => setEmail(e.target.value)}
              required
            />

            <Button type="submit" className="w-full">
              Envoyer le lien
            </Button>

            <Link
              to="/login"
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
