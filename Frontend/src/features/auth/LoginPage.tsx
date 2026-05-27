import { useState } from "react";
import { useAuthContext } from "./AuthContext";
import { useNavigate, useLocation } from "react-router-dom";

export default function LoginPage() {
  const { login } = useAuthContext();
  const navigate = useNavigate();
  const location = useLocation();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  // 🔥 returnTo : si on vient de /boards/123 → on y retourne après login
  const from = location.state?.from || "/";

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    try {
        await login(email, password);
        navigate(from);
    } catch (err: any) {
        setError(err.message || "Erreur de connexion");
    }
  };

  return (
    <div style={{ padding: 20 }}>
      <h2>Connexion</h2>

      {error && <p style={{ color: "red" }}>{error}</p>}

      <form onSubmit={submit}>
        <input
          placeholder="Email"
          value={email}
          onChange={e => setEmail(e.target.value)}
          style={{ display: "block", marginBottom: 10 }}
        />

        <input
          placeholder="Mot de passe"
          type="password"
          value={password}
          onChange={e => setPassword(e.target.value)}
          style={{ display: "block", marginBottom: 10 }}
        />

        <button type="submit">Se connecter</button>
      </form>
    </div>
  );
}
