namespace TaskBoard.Application.Common.Emails;

public static class EmailTemplates
{
    private const string BrandColor = "#4f46e5"; // Indigo-600
    private const string TextColor = "#1f2937";  // Gray-800
    private const string MutedColor = "#6b7280"; // Gray-500

    private static string Wrap(string title, string content)
    {
        return $@"
        <div style='font-family: Arial, sans-serif; max-width: 480px; margin: auto; padding: 24px; border-radius: 12px; border: 1px solid #e5e7eb;'>
            <h2 style='color: {TextColor}; text-align: center; margin-bottom: 16px;'>{title}</h2>
            <div style='color: {TextColor}; font-size: 15px; line-height: 1.6;'>
                {content}
            </div>
            <p style='text-align: center; margin-top: 32px; color: {MutedColor}; font-size: 13px;'>
                TaskBoard — Gestion de tâches professionnelle
            </p>
        </div>";
    }

    // -------------------------------------------------------------------------
    // CONFIRM EMAIL
    // -------------------------------------------------------------------------
    public static string ConfirmEmail(string displayName, string confirmLink)
    {
        var content = $@"
            <p>Bonjour <strong>{displayName}</strong>,</p>
            <p>Merci de vous être inscrit à TaskBoard.</p>
            <p>Cliquez sur le bouton ci‑dessous pour confirmer votre adresse email :</p>

            <div style='text-align: center; margin: 24px 0;'>
                <a href='{confirmLink}' 
                   style='background: {BrandColor}; color: white; padding: 12px 24px; 
                          border-radius: 8px; text-decoration: none; font-weight: bold;'>
                    Confirmer mon email
                </a>
            </div>

            <p>Si vous n'avez pas créé de compte, vous pouvez ignorer cet email.</p>
        ";

        return Wrap("Confirmez votre email", content);
    }

    // -------------------------------------------------------------------------
    // RESET PASSWORD
    // -------------------------------------------------------------------------
    public static string ResetPassword(string resetLink)
    {
        var content = $@"
            <p>Vous avez demandé une réinitialisation de mot de passe.</p>
            <p>Cliquez sur le bouton ci‑dessous pour définir un nouveau mot de passe :</p>

            <div style='text-align: center; margin: 24px 0;'>
                <a href='{resetLink}' 
                   style='background: {BrandColor}; color: white; padding: 12px 24px; 
                          border-radius: 8px; text-decoration: none; font-weight: bold;'>
                    Réinitialiser mon mot de passe
                </a>
            </div>

            <p>Si vous n'êtes pas à l'origine de cette demande, vous pouvez ignorer cet email.</p>
        ";

        return Wrap("Réinitialisation du mot de passe", content);
    }

    // -------------------------------------------------------------------------
    // WELCOME EMAIL
    // -------------------------------------------------------------------------
    public static string Welcome(string displayName)
    {
        var content = $@"
            <p>Bienvenue <strong>{displayName}</strong> 🎉</p>
            <p>Votre compte TaskBoard est maintenant actif.</p>
            <p>Vous pouvez commencer à organiser vos tâches, vos projets, et vos tableaux.</p>

            <div style='text-align: center; margin: 24px 0;'>
                <a href='https://taskboard.app' 
                   style='background: {BrandColor}; color: white; padding: 12px 24px; 
                          border-radius: 8px; text-decoration: none; font-weight: bold;'>
                    Accéder à TaskBoard
                </a>
            </div>
        ";

        return Wrap("Bienvenue sur TaskBoard", content);
    }

    // -------------------------------------------------------------------------
    // PASSWORD CHANGED
    // -------------------------------------------------------------------------
    public static string PasswordChanged()
    {
        var content = @"
            <p>Votre mot de passe a été mis à jour avec succès.</p>
            <p>Si vous n'êtes pas à l'origine de cette action, changez immédiatement votre mot de passe et contactez le support.</p>
        ";

        return Wrap("Mot de passe mis à jour", content);
    }
}
