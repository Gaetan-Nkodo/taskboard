import { useState } from "react";
import { Link } from "react-router-dom";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { useApiClient } from "@/core/api/apiClient";
import { useAuthContext } from "@/features/auth/AuthProvider";

export default function ConfirmEmailSentPage() {
  const api = useApiClient();
  const { user } = useAuthContext();

  const [status, setStatus] = useState<"idle" | "sent" | "error">("idle");

  async function resend() {
    const email = localStorage.getItem("pendingEmail");
    if (!email) {
      setStatus("error");
      return;
    }

    try {
      await api.post("/api/v1/auth/resend-confirmation", {email});
      setStatus("sent");
    } catch {
      setStatus("error");
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center px-4 animate-fadeIn">
      <Card className="w-full max-w-md p-6 shadow-xl border border-border/40 bg-card/80 backdrop-blur-md">
        <CardHeader className="text-center">
          <CardTitle className="text-xl font-bold">Vérifiez votre email</CardTitle>
        </CardHeader>

        <CardContent className="text-center space-y-4">
          <p className="text-muted-foreground">
            Un lien de confirmation vous a été envoyé.
            Cliquez dessus pour activer votre compte.
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

          <Link
            to="/login"
            className="text-primary hover:underline text-sm block mt-2"
          >
            Retour à la connexion
          </Link>
        </CardContent>
      </Card>
    </div>
  );
}
