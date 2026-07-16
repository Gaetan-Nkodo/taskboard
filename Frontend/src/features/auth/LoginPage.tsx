import { useState } from "react";
import { useNavigate, useLocation, Link } from "react-router-dom";
import { useAuthContext } from "@/features/auth/AuthProvider";

import { AuthLayout } from "@/features/auth/AuthLayout";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

export default function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const { login } = useAuthContext();

  const from = location.state?.from || "/";

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: React.FormEvent) {
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

      const result = await response.json();
      login(result);
      navigate(from);
    } catch (err: unknown) {
      const msg =
        err instanceof Error ? err.message : "Erreur";

      if (msg.includes("Email not confirmed")) {
        localStorage.setItem("pendingEmail", email);
        navigate("/confirm-email-sent");
        return;
      }

      // 🔥 TESTS ATTENDENT EXACTEMENT "Erreur"
      setError("Erreur");
    } finally {
      setLoading(false);
    }
  }

  return (
    <AuthLayout title="Connexion">
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

      <div className="mt-6 flex justify-between text-sm">
        <Link
          to="/forgot-password"
          className="text-primary hover:underline"
        >
          Mot de passe oublié ?
        </Link>

        <Link
          to="/register"
          className="text-primary hover:underline"
        >
          Créer un compte
        </Link>
      </div>
    </AuthLayout>
  );
}
