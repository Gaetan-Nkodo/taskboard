import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { FadeIn } from "@/components/ui/FadeIn";

export function AuthLayout({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <div className="min-h-screen flex items-center justify-center px-4 animate-fadeIn">
      <Card className="w-full max-w-md p-6 shadow-xl border border-border/40 bg-card/80 backdrop-blur-md">
        <CardHeader className="text-center">
          <CardTitle className="text-xl font-bold">{title}</CardTitle>
        </CardHeader>

        <CardContent className="space-y-4">
          {children}
        </CardContent>
      </Card>
    </div>
  );
}
