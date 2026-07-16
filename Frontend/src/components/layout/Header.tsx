import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Bell, Menu, X } from "lucide-react";

import { useAuthContext } from "@/features/auth/AuthProvider";
import { ThemeToggle } from "@/components/theme/ThemeToggle";

import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuTrigger,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
} from "@/components/ui/dropdown-menu";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";

export function Header() {
  const { user, logout } = useAuthContext();
  const navigate = useNavigate();
  const [mobileOpen, setMobileOpen] = useState(false);

  if (!user) return null;

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <header className="sticky top-0 z-40 w-full border-b bg-background/80 backdrop-blur">
      <div className="mx-auto flex h-16 max-w-6xl items-center justify-between px-4 md:px-6">

        {/* Logo */}
        <Link
          to="/"
          className="flex items-center gap-2 text-lg font-semibold tracking-tight text-primary hover:text-primary/80 transition-colors"
        >
          <span className="inline-flex h-7 w-7 items-center justify-center rounded-lg bg-primary/10 text-primary text-sm font-bold">
            TB
          </span>
          <span>TaskBoard</span>
        </Link>

        {/* Desktop Navigation */}
        <div className="hidden items-center gap-4 md:flex">
          <nav className="flex items-center gap-3 text-sm text-muted-foreground">
            <Link to="/boards" className="nav-link">Boards</Link>
          </nav>

          {/* Notifications */}
          <Button variant="ghost" size="icon" className="relative">
            <Bell className="h-4 w-4" />
            <span className="notif-badge">3</span>
          </Button>

          <ThemeToggle />

          {/* User Menu */}
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button
                variant="outline"
                className="flex items-center gap-2 rounded-full border-border/70 bg-background/60 px-3 py-1.5 text-sm shadow-sm hover:bg-muted/70"
              >
                <Avatar className="h-7 w-7">
                  <AvatarFallback>{user.displayName?.[0]?.toUpperCase()}</AvatarFallback>
                </Avatar>
                <span className="max-w-[140px] truncate">{user.displayName}</span>
              </Button>
            </DropdownMenuTrigger>

            <DropdownMenuContent align="end" className="w-64 p-2 rounded-xl shadow-xl border border-border/60">
              <div className="px-3 py-2">
                <div className="text-xs text-muted-foreground">Connecté en tant que</div>
                <div className="truncate font-semibold">{user.email}</div>
              </div>

              <DropdownMenuSeparator />

              <DropdownMenuItem asChild>
                <Link to="/change-password" className="flex items-center gap-2">
                  🔐 Changer mot de passe
                </Link>
              </DropdownMenuItem>

              <DropdownMenuSeparator />

              <DropdownMenuItem
                className="text-red-600 font-medium focus:text-red-700 flex items-center gap-2"
                onClick={handleLogout}
              >
                🚪 Se déconnecter
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>

        {/* Mobile Navigation */}
        <div className="flex items-center gap-2 md:hidden">
          <Button variant="ghost" size="icon" className="relative">
            <Bell className="h-4 w-4" />
            <span className="notif-badge-mobile">3</span>
          </Button>

          <button
            className="mobile-menu-btn"
            onClick={() => setMobileOpen((v) => !v)}
          >
            {mobileOpen ? <X className="h-4 w-4" /> : <Menu className="h-4 w-4" />}
          </button>
        </div>
      </div>
    </header>
  );
}
