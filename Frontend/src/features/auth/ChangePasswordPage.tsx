import { useState } from "react";
import { useAuth } from "../../features/auth/useAuth";
import toast from "react-hot-toast";

export function ChangePasswordPage() {
  const { getToken, logout } = useAuth();

  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirm, setConfirm] = useState("");

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();

    if (newPassword !== confirm) {
      toast.error("Les mots de passe ne correspondent pas.");
      return;
    }

    const response = await fetch("/api/v1/auth/change-password", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + getToken()
      },
      body: JSON.stringify({
        currentPassword,
        newPassword
      })
    });

    if (response.status === 200) {
      toast.success("Mot de passe mis à jour !");
      setCurrentPassword("");
      setNewPassword("");
      setConfirm("");
      return;
    }

    if (response.status === 400) {
      toast.error("Mot de passe actuel incorrect.");
      return;
    }

    if (response.status === 401) {
      toast.error("Session expirée.");
      logout();
      return;
    }

    toast.error("Une erreur est survenue.");
  }

  return (
    <div className="max-w-sm mx-auto mt-20 p-6 bg-white shadow rounded">
      <h1 className="text-xl font-bold mb-4">Changer le mot de passe</h1>

      <form onSubmit={handleSubmit} className="space-y-3">
        <input
          type="password"
          placeholder="Mot de passe actuel"
          className="w-full border p-2 rounded"
          value={currentPassword}
          onChange={e => setCurrentPassword(e.target.value)}
          required
        />

        <input
          type="password"
          placeholder="Nouveau mot de passe"
          className="w-full border p-2 rounded"
          value={newPassword}
          onChange={e => setNewPassword(e.target.value)}
          required
        />

        <input
          type="password"
          placeholder="Confirmer le mot de passe"
          className="w-full border p-2 rounded"
          value={confirm}
          onChange={e => setConfirm(e.target.value)}
          required
        />

        <button
          type="submit"
          className="w-full bg-blue-600 text-white p-2 rounded"
        >
          Mettre à jour
        </button>
      </form>
    </div>
  );
}
