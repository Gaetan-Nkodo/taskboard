import { useState } from "react";
import { Link } from "react-router-dom";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { useApiClient } from "@/core/api/apiClient";
import { useAuthContext } from "@/features/auth/AuthProvider";

export default function ConfirmYourEmailPage() {
  const api = useApiClient();
  const { user } = useAuthContext();
  const [status, setStatus] = useState<"idle" | "sent" | "error">("idle");

  async function resend() {
    try {
      await api.post("/api/v1/auth/resend-confirmation", {
        email: user?.email
      });
      setStatus("sent");
    } catch {
      setStatus("error");
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center px-4">
      <Card className="w-full max-w-md p-6 shadow-xl">
        <CardHeader className="text-center">
          <CardTitle className="text-xl font-bold">Email non confirmé</CardTitle>
        </CardHeader>

        <CardContent className="text-center space-y-4">
          <p className="text-muted-foreground">
            Vous devez confirmer votre email avant de pouvoir vous connecter.
          </p>

          {status === "sent" && (
            <p className="text-green-600">Email de confirmation renvoyé !</p>
          )}

          {status === "error" && (
            <p className="text-red-600">Impossible de renvoyer l’email.</p>
          )}

          <Button className="w-full" onClick={resend}>
            Renvoyer l’email de confirmation
          </Button>

          <Link to="/login" className="text-primary hover:underline text-sm">
            Retour à la connexion
          </Link>
        </CardContent>
      </Card>
    </div>
  );
}
