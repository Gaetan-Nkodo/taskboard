import { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { useAuth } from "./useAuth";
import { useAuthContext } from "./AuthProvider";

export default function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();

  const { login } = useAuth();
  
  const returnTo = location.state?.from || "/boards";

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");
  const { applyTokens } = useAuthContext();

  // 🔥 Lecture du message d’expiration
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
      setError("Invalid credentials");
    }
  }

  return (
    <div className="max-w-sm mx-auto mt-20 p-6 bg-white shadow rounded">
      <h1 className="text-xl font-bold mb-4">Connexion</h1>

      {/* 🔥 Message d’expiration */}
      {message && <div className="text-blue-600 mb-2">{message}</div>}

      {/* Erreur login */}
      {error && <div className="text-red-600 mb-2">{error}</div>}

      <form
        role="form"
        data-testid="login-form"
        onSubmit={handleSubmit}
        className="space-y-3"
      >
        <input
          type="email"
          placeholder="Email"
          className="w-full border p-2 rounded"
          value={email}
          onChange={e => setEmail(e.target.value)}
        />

        <input
          type="password"
          placeholder="Mot de passe"
          className="w-full border p-2 rounded"
          value={password}
          onChange={e => setPassword(e.target.value)}
        />

        <button
          type="submit"
          className="w-full bg-blue-600 text-white p-2 rounded"
        >
          Se connecter
        </button>
        <div className="text-right">
          <a href="/forgot-password" className="text-blue-600 text-sm">
            Mot de passe oublié ?
          </a>
        </div>
      </form>
    </div>
  );
}
