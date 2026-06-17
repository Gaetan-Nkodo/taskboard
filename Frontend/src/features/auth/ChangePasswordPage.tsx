import { useState } from "react";
import { Link } from "react-router-dom";
import toast from "react-hot-toast";

import { useAuthContext } from "@/features/auth/AuthProvider";
import { useHttp } from "@/core/api/httpClient";

import { Card } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

export function ChangePasswordPage() {
  const http = useHttp();
  const { logout } = useAuthContext();

  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirm, setConfirm] = useState("");

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();

    if (newPassword !== confirm) {
      toast.error("Les mots de passe ne correspondent pas.");
      return;
    }

    try {
      await http("/api/v1/auth/change-password", {
        method: "POST",
        body: JSON.stringify({
          currentPassword,
          newPassword
        })
      });

      toast.success("Mot de passe mis à jour !");
      setCurrentPassword("");
      setNewPassword("");
      setConfirm("");
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : String(err) ?? "";

      if (msg.includes("401") || msg.includes("expired")) {
        toast.error("Session expirée.");
        logout();
        return;
      }

      if (msg.includes("400")) {
        toast.error("Mot de passe actuel incorrect.");
        return;
      }

      toast.error("Une erreur est survenue.");
    }
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
            onChange={(e) => setCurrentPassword(e.target.value)}
            required
          />

          <Input
            type="password"
            placeholder="Nouveau mot de passe"
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
            required
          />

          <Input
            type="password"
            placeholder="Confirmer le mot de passe"
            value={confirm}
            onChange={(e) => setConfirm(e.target.value)}
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
