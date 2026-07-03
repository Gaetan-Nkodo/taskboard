import { useEffect, useState } from "react";
import { useSearchParams, Link } from "react-router-dom";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { useApiClient } from "@/core/api/apiClient";

export default function ConfirmEmailPage() {
  const api = useApiClient();
  const [params] = useSearchParams();
  const token = params.get("token");

  const [status, setStatus] = useState<"loading" | "success" | "error">("loading");

  useEffect(() => {
    async function confirm() {
      try {
        await api.get(`/api/v1/auth/confirm-email?token=${token}`);
        setStatus("success");
      } catch {
        setStatus("error");
      }
    }

    confirm();
  }, [token]);

  return (
    <div className="min-h-screen flex items-center justify-center px-4 animate-fadeIn">
      <Card className="w-full max-w-md p-6 shadow-xl border border-border/40 bg-card/80 backdrop-blur-md">
        <CardHeader className="text-center">
          <CardTitle className="text-xl font-bold">
            {status === "loading" && "Confirmation en cours..."}
            {status === "success" && "Email confirmé 🎉"}
            {status === "error" && "Lien invalide ou expiré"}
          </CardTitle>
        </CardHeader>

        <CardContent className="text-center space-y-4">
          {status === "success" && (
            <>
              <p className="text-green-600">Votre email a été confirmé avec succès.</p>
              <Button asChild className="w-full">
                <Link to="/login">Se connecter</Link>
              </Button>
            </>
          )}

          {status === "error" && (
            <>
              <p className="text-red-600">Impossible de confirmer votre email.</p>
              <Button asChild className="w-full">
                <Link to="/register">Créer un compte</Link>
              </Button>
            </>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
