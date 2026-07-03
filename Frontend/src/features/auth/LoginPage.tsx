import { useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { useAuthContext } from "@/features/auth/AuthProvider";

import { Card, Input, Button } from "@/components/ui";
import { LoginResponse } from "@/core/models/Auth";

export default function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const { login } = useAuthContext();

  const from = location.state?.from || "/";

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      const response = await fetch(
        `${import.meta.env.VITE_API_URL}/api/v1/auth/login`,
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ email, password })
        }
      );

      if (!response.ok) {
        throw new Error(await response.text());
      }

      const result: LoginResponse = await response.json();
      login(result);
      navigate(from);
    } catch (err: unknown) {

      const msg = err instanceof Error ? err.message : String(err) ?? "Erreur de connexion";
      const status = (err as { status?: number }).status;
      if (msg.includes("Email not confirmed") || status === 403) {
          setError("Veuillez confirmer votre email avant de vous connecter.");
          navigate("/confirm-your-email");
          return;
      }
      setError(msg);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center px-4">
      <Card className="w-full max-w-md p-6 relative">
        <h1 className="text-2xl font-bold mb-6 text-center">Connexion</h1>

        {error && (
          <div className="mb-4 text-red-600 text-sm">{error}</div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label htmlFor="email" className="text-sm font-medium">
              Email
            </label>
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
            <label htmlFor="password" className="text-sm font-medium">
              Mot de passe
            </label>
            <Input
              id="password"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              className="mt-1"
            />
          </div>

          <Button
            type="submit"
            variant="default"
            disabled={loading}
            className="w-full"
          >
            {loading ? "Connexion..." : "Se connecter"}
          </Button>
        </form>

        {/* 🔥 Liens en bas gauche et bas droite */}
        <div className="mt-6 flex justify-between text-sm">
          <a
            href="/forgot-password"
            className="text-blue-600 hover:underline"
          >
            Mot de passe oublié ?
          </a>

          <a
            href="/register"
            className="text-primary hover:underline"
          >
            Créer un compte
          </a>
        </div>
      </Card>
    </div>
  );
}
