import { useState } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "@/features/auth/useAuth";
import toast from "react-hot-toast";

import { Card, Input, Button } from "@/components/ui";

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
    <div className="min-h-screen flex items-center justify-center px-4 sm:px-6 lg:px-8">
      <Card className="w-full max-w-sm p-6 animate-fadeIn">
        <h1 className="text-xl font-bold mb-4">Changer le mot de passe</h1>

        <form onSubmit={handleSubmit} className="space-y-4">
          <Input
            type="password"
            placeholder="Mot de passe actuel"
            value={currentPassword}
            onChange={e => setCurrentPassword(e.target.value)}
            required
          />

          <Input
            type="password"
            placeholder="Nouveau mot de passe"
            value={newPassword}
            onChange={e => setNewPassword(e.target.value)}
            required
          />

          <Input
            type="password"
            placeholder="Confirmer le mot de passe"
            value={confirm}
            onChange={e => setConfirm(e.target.value)}
            required
          />

          <Button type="submit" className="w-full">
            Mettre à jour
          </Button>

          <Link
            to="/"
            className="block text-center text-sm text-muted-foreground hover:text-primary transition-colors"
          >
            Retour au tableau de bord
          </Link>
        </form>
      </Card>
    </div>
  );
}
