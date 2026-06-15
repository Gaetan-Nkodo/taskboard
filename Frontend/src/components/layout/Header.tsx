import { useState } from "react";
import { Link } from "react-router-dom";
import { Bell, Menu, X } from "lucide-react";

import { useAuth } from "@/features/auth/useAuth";
import { ThemeToggle } from "@/components/theme/ThemeToggle";

import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuTrigger,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator
} from "@/components/ui/dropdown-menu";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";

export function Header() {
  const { getUser, logout } = useAuth();
  const user = getUser();
  const [mobileOpen, setMobileOpen] = useState(false);

  if (!user) return null;

  return (
    <header className="sticky top-0 z-40 w-full border-b bg-background/80 backdrop-blur supports-[backdrop-filter]:bg-background/60">
      <div className="mx-auto flex h-16 max-w-6xl items-center justify-between px-4 md:px-6">
        {/* Logo / Brand */}
        <Link
          to="/"
          className="flex items-center gap-2 text-lg font-semibold tracking-tight text-primary hover:text-primary/80 transition-colors"
        >
          <span className="inline-flex h-7 w-7 items-center justify-center rounded-lg bg-primary/10 text-primary text-sm font-bold">
            TB
          </span>
          <span>TaskBoard</span>
        </Link>

        {/* Desktop actions */}
        <div className="hidden items-center gap-4 md:flex">
          {/* Navigation simple (extensible) */}
          <nav className="flex items-center gap-3 text-sm text-muted-foreground">
            <Link
              to="/boards"
              className="rounded-md px-2 py-1 hover:bg-muted hover:text-foreground transition-colors"
            >
              Boards
            </Link>
            <Link
              to="/stats"
              className="rounded-md px-2 py-1 hover:bg-muted hover:text-foreground transition-colors"
            >
              Statistiques
            </Link>
          </nav>

          {/* Notifications */}
          <Button
            variant="ghost"
            size="icon"
            className="relative"
            aria-label="Notifications"
          >
            <Bell className="h-4 w-4" />
            <span className="absolute -right-0.5 -top-0.5 inline-flex h-4 min-w-[1rem] items-center justify-center rounded-full bg-red-500 px-1 text-[10px] font-medium text-white shadow-sm">
              3
            </span>
          </Button>

          {/* Theme toggle */}
          <ThemeToggle />

          {/* User menu */}
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button
                variant="outline"
                className="flex items-center gap-2 rounded-full border-border/70 bg-background/60 px-2.5 py-1.5 text-sm shadow-sm hover:bg-muted/70"
              >
                <Avatar className="h-7 w-7">
                  <AvatarFallback>
                    {user.displayName?.[0]?.toUpperCase()}
                  </AvatarFallback>
                </Avatar>
                <span className="max-w-[140px] truncate">
                  {user.displayName}
                </span>
              </Button>
            </DropdownMenuTrigger>

            <DropdownMenuContent align="end" className="w-56">
              <DropdownMenuItem disabled className="text-xs text-muted-foreground">
                Connecté en tant que
                <span className="block truncate font-medium text-foreground">
                  {user.email}
                </span>
              </DropdownMenuItem>

              <DropdownMenuSeparator />

              <DropdownMenuItem asChild>
                <Link to="/change-password">Changer mot de passe</Link>
              </DropdownMenuItem>

              <DropdownMenuSeparator />

              <DropdownMenuItem
                className="text-red-600 focus:text-red-700"
                onClick={logout}
              >
                Se déconnecter
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>

        {/* Mobile actions */}
        <div className="flex items-center gap-2 md:hidden">
          <Button
            variant="ghost"
            size="icon"
            className="relative"
            aria-label="Notifications"
          >
            <Bell className="h-4 w-4" />
            <span className="absolute -right-0.5 -top-0.5 inline-flex h-3.5 min-w-[0.9rem] items-center justify-center rounded-full bg-red-500 px-1 text-[9px] font-medium text-white shadow-sm">
              3
            </span>
          </Button>

          <button
            className="inline-flex h-9 w-9 items-center justify-center rounded-md border border-border/70 bg-background/70 text-foreground shadow-sm hover:bg-muted transition-colors"
            onClick={() => setMobileOpen((v) => !v)}
            aria-label="Ouvrir le menu"
          >
            {mobileOpen ? <X className="h-4 w-4" /> : <Menu className="h-4 w-4" />}
          </button>
        </div>
      </div>

      {/* Mobile menu */}
      {mobileOpen && (
        <div className="md:hidden animate-fadeIn border-t bg-background/95 backdrop-blur">
          <div className="mx-auto flex max-w-6xl flex-col gap-3 px-4 py-3 text-sm">
            <nav className="flex flex-col gap-1 text-muted-foreground">
              <Link
                to="/boards"
                className="rounded-md px-2 py-1 hover:bg-muted hover:text-foreground transition-colors"
                onClick={() => setMobileOpen(false)}
              >
                Boards
              </Link>
              <Link
                to="/stats"
                className="rounded-md px-2 py-1 hover:bg-muted hover:text-foreground transition-colors"
                onClick={() => setMobileOpen(false)}
              >
                Statistiques
              </Link>
            </nav>

            <div className="flex items-center justify-between pt-2">
              <div className="flex items-center gap-2">
                <Avatar className="h-8 w-8">
                  <AvatarFallback>
                    {user.displayName?.[0]?.toUpperCase()}
                  </AvatarFallback>
                </Avatar>
                <div className="flex flex-col">
                  <span className="text-sm font-medium">{user.displayName}</span>
                  <span className="text-xs text-muted-foreground truncate max-w-[180px]">
                    {user.email}
                  </span>
                </div>
              </div>

              <ThemeToggle />
            </div>

            <button
              onClick={() => {
                setMobileOpen(false);
                logout();
              }}
              className="mt-1 w-full rounded-md bg-red-500/10 px-3 py-2 text-left text-sm font-medium text-red-600 hover:bg-red-500/15 transition-colors"
            >
              Se déconnecter
            </button>
          </div>
        </div>
      )}
    </header>
  );
}
