import { useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { AuthService } from "../../core/services/AuthService";
import { useAuthContext } from "./AuthProvider";

export default function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const { setUser, setToken } = useAuthContext();

  const returnTo = location.state?.from || "/boards";

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError("");

    try {
      const result = await AuthService.login(email, password);
      setUser(result.user);
      setToken(result.token);
      navigate(returnTo);
    } catch (err: any) {
      setError("Invalid credentials");
    }
  }

  return (
    <div className="max-w-sm mx-auto mt-20 p-6 bg-white shadow rounded">
      <h1 className="text-xl font-bold mb-4">Connexion</h1>

      {error && <div className="text-red-600 mb-2">{error}</div>}

      <form role="form" onSubmit={handleSubmit} className="space-y-3">
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
      </form>
    </div>
  );
}
