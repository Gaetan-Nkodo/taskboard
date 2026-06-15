import { Outlet } from "react-router-dom";
import { Header } from "./Header";
import { Container } from "./Container";

export function Layout() {
  return (
    <div className="min-h-screen bg-[var(--color-bg)] text-[var(--color-text)]">
      <Header />
      <Container>
        <Outlet />
      </Container>
    </div>
  );
}
