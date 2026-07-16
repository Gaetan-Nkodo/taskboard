import { Outlet } from "react-router-dom";
import { Header } from "./Header";

export function Layout() {
  return (
    <div className="min-h-screen flex flex-col bg-background text-foreground">
      <Header />

      <main className="flex-1 mx-auto w-full max-w-6xl px-4 py-6 md:px-6">
        <Outlet />
      </main>
    </div>
  );
}
