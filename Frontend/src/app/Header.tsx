import { useAuthContext } from "../features/auth/AuthProvider";
import { Link, useNavigate } from "react-router-dom";

export default function Header() {
  const { user, logout } = useAuthContext();
  const navigate = useNavigate();

  if (!user) return null;

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <header
      style={{
        padding: "10px 20px",
        background: "#f5f5f5",
        marginBottom: "5px",
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        borderBottom: "1px solid #ddd"
      }}
    >
      <strong style={{ fontSize: "18px" }}>TaskBoard</strong>

      <div style={{ display: "flex", alignItems: "center", gap: "15px" }}>
        <span>Bonjour {user.displayName}</span>

        <Link
          to="/change-password"
          style={{
            color: "#0070f3",
            textDecoration: "none",
            fontWeight: "500"
          }}
        >
          Changer mot de passe
        </Link>

        <button
          onClick={handleLogout}
          style={{
            background: "#e63946",
            color: "white",
            border: "none",
            padding: "6px 12px",
            borderRadius: "4px",
            cursor: "pointer"
          }}
        >
          Se déconnecter
        </button>
      </div>
    </header>
  );
}
