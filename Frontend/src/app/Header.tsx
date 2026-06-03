import { useAuthContext } from "../features/auth/AuthProvider";
import { useNavigate } from "react-router-dom";

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
        alignItems: "center"
      }}
    >
      <strong>TaskBoard</strong>

      <div>
        <span style={{ marginRight: "15px" }}>
          Bonjour {user.displayName}
        </span>
        <button onClick={handleLogout}>Se déconnecter</button>
      </div>
    </header>
  );
}
