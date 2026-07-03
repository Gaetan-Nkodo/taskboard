import { Link } from "react-router-dom";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";

export default function ConfirmEmailSentPage() {
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

          <Link
            to="/login"
            className="text-primary hover:underline text-sm"
          >
            Retour à la connexion
          </Link>
        </CardContent>
      </Card>
    </div>
  );
}
