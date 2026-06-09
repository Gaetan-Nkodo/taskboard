import { useState } from "react";
import { useSearchParams } from "react-router-dom";

export default function ResetPasswordPage() {
  const [params] = useSearchParams();
  const token = params.get("token");

  const [password, setPassword] = useState("");
  const [done, setDone] = useState(false);
  const [error, setError] = useState("");

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError("");

    const res = await fetch("/api/v1/auth/reset-password", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ token, newPassword: password })
    });

    if (res.status === 401) {
      setError("Lien invalide ou expiré.");
      return;
    }

    setDone(true);
  }

  return (
    <div className="max-w-sm mx-auto mt-20 p-6 bg-white shadow rounded">
      <h1 className="text-xl font-bold mb-4">Réinitialiser le mot de passe</h1>

      {done ? (
        <div className="text-green-600">
          Mot de passe mis à jour. Vous pouvez maintenant vous connecter.
        </div>
      ) : (
        <form onSubmit={handleSubmit} className="space-y-3">
          {error && <div className="text-red-600">{error}</div>}

          <input
            type="password"
            placeholder="Nouveau mot de passe"
            className="w-full border p-2 rounded"
            value={password}
            onChange={e => setPassword(e.target.value)}
            required
          />

          <button
            type="submit"
            className="w-full bg-blue-600 text-white p-2 rounded"
          >
            Mettre à jour
          </button>
        </form>
      )}
    </div>
  );
}
