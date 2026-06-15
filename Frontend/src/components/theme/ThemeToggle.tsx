import { useTheme } from "@/components/theme/useTheme";
import { Switch } from "@/components/ui/switch";

export function ThemeToggle() {
  const { theme, toggleTheme } = useTheme();

  return (
    <div className="flex items-center gap-2">
      <span className="text-sm">{theme === "light" ? "☀️" : "🌙"}</span>
      <Switch checked={theme === "dark"} onCheckedChange={toggleTheme} />
    </div>
  );
}
