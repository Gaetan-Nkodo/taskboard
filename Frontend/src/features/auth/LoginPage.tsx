import { useState, useEffect } from "react";
import { useNavigate, useLocation, Link } from "react-router-dom";

import { useAuth } from "@/features/auth/useAuth";
import { useAuthContext } from "@/features/auth/AuthProvider";

import { Card, Input, Button } from "@/components/ui";

export default function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();

  const { login } = useAuth();
  const { applyTokens } = useAuthContext();

  const returnTo = location.state?.from || "/boards";

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [error, setError] = useState("");
  const [message, setMessage] = useState("");

  // 🔥 Message d’expiration
  useEffect(() => {
    const reason = localStorage.getItem("logoutReason");

    if (reason === "expired") {
      setMessage("Votre session a expiré, veuillez vous reconnecter");
      localStorage.removeItem("logoutReason");
    }
  }, []);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError("");

    try {
      const result = await login({ email, password });
      applyTokens(result);
      navigate(returnTo);
    } catch {
      setError("Identifiants invalides");
    }
  }

  return (
    <div className="login-bg min-h-screen flex items-center justify-center px-4 sm:px-6 lg:px-8">
      <Card className="w-full max-w-sm p-6 animate-fadeIn">
        <h1 className="text-xl font-bold mb-4">Connexion</h1>

        {message && (
          <div className="mb-2 text-sm text-blue-600">{message}</div>
        )}

        {error && (
          <div className="mb-2 text-sm text-red-600">{error}</div>
        )}

        <form
          role="form"
          data-testid="login-form"
          onSubmit={handleSubmit}
          className="space-y-4"
        >
          <Input
            type="email"
            placeholder="Email"
            value={email}
            onChange={e => setEmail(e.target.value)}
            required
          />

          <Input
            type="password"
            placeholder="Mot de passe"
            value={password}
            onChange={e => setPassword(e.target.value)}
            required
          />

          <Button type="submit" className="w-full">
            Se connecter
          </Button>

          <div className="flex justify-between text-sm">
            <Link
              to="/forgot-password"
              className="text-primary hover:underline"
            >
              Mot de passe oublié
            </Link>

            {/* Si un jour tu ajoutes l'inscription */}
            {/* <Link to="/register" className="text-primary hover:underline">
              Créer un compte
            </Link> */}
          </div>
        </form>
      </Card>
    </div>
  );
}
