import { ReactElement } from "react";
import { motion } from "framer-motion";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  BarChart,
  Bar,
  Legend,
} from "recharts";

import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { KanbanSquare, Users, Flame, Sparkles, Clock } from "lucide-react";

type DashboardCardProps = {
  icon: ReactElement;
  title: string;
  value: string;
  trend: string;
};

type ActionButtonProps = {
  label: string;
};

type SpotlightBoardProps = {
  title: string;
  tasks: number;
};

type TimelineItemProps = {
  text: string;
  time: string;
};

const productivityData = [
  { week: "Semaine 1", completed: 18 },
  { week: "Semaine 2", completed: 24 },
  { week: "Semaine 3", completed: 32 },
  { week: "Semaine 4", completed: 42 },
];

const statusDistributionData = [
  { status: "À faire", count: 34 },
  { status: "En cours", count: 29 },
  { status: "En revue", count: 12 },
  { status: "Terminé", count: 87 },
];

export function DashboardPage() {
  return (
    <div className="space-y-10">
      {/* Header Section */}
      <motion.div
        initial={{ opacity: 0, y: -10 }}
        animate={{ opacity: 1, y: 0 }}
        className="space-y-2"
      >
        <h1 className="text-4xl font-bold tracking-tight flex items-center gap-3">
          <Sparkles className="h-7 w-7 text-primary" />
          Dashboard
        </h1>
        <p className="text-muted-foreground text-lg">
          Vue d’ensemble de votre productivité, vos boards et votre équipe.
        </p>
      </motion.div>

      {/* KPI Cards */}
      <motion.div
        initial={{ opacity: 0 }}
        animate={{ opacity: 1 }}
        className="grid gap-6 md:grid-cols-3"
      >
        <DashboardCard
          icon={<KanbanSquare className="h-6 w-6 text-primary" />}
          title="Boards"
          value="12"
          trend="+2 ce mois"
        />

        <DashboardCard
          icon={<Flame className="h-6 w-6 text-primary" />}
          title="Tâches actives"
          value="87"
          trend="+14 cette semaine"
        />

        <DashboardCard
          icon={<Users className="h-6 w-6 text-primary" />}
          title="Collaborateurs"
          value="5"
          trend="Stable"
        />
      </motion.div>

      {/* Charts Row */}
      <div className="grid gap-6 lg:grid-cols-2">
        {/* Productivity Line Chart */}
        <Card className="shadow-sm border-border/60">
          <CardHeader>
            <CardTitle className="text-lg">Tâches complétées par semaine</CardTitle>
          </CardHeader>
          <CardContent className="h-64">
            <ResponsiveContainer width="100%" height="100%">
              <LineChart data={productivityData} margin={{ top: 10, right: 20, left: 0, bottom: 0 }}>
                <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
                <XAxis dataKey="week" tick={{ fontSize: 12 }} />
                <YAxis tick={{ fontSize: 12 }} />
                <Tooltip />
                <Line
                  type="monotone"
                  dataKey="completed"
                  stroke="#3b82f6"
                  strokeWidth={2}
                  dot={{ r: 3 }}
                />
              </LineChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>

        {/* Status Distribution Bar Chart */}
        <Card className="shadow-sm border-border/60">
          <CardHeader>
            <CardTitle className="text-lg">Répartition des tâches par statut</CardTitle>
          </CardHeader>
          <CardContent className="h-64">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={statusDistributionData} margin={{ top: 10, right: 20, left: 0, bottom: 0 }}>
                <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
                <XAxis dataKey="status" tick={{ fontSize: 12 }} />
                <YAxis tick={{ fontSize: 12 }} />
                <Tooltip />
                <Legend />
                <Bar dataKey="count" fill="#22c55e" radius={[4, 4, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>
      </div>

      {/* Quick Actions + Spotlight */}
      <div className="grid gap-6 lg:grid-cols-2">
        <QuickActions />
        <SpotlightBoards />
      </div>

      {/* Activity + Summary */}
      <div className="grid gap-6 lg:grid-cols-2">
        <ActivityTimeline />
        <ProductivitySummary />
      </div>
    </div>
  );
}

/* ---------------- COMPONENTS ---------------- */

function DashboardCard({ icon, title, value, trend }: DashboardCardProps) {
  return (
    <Card className="shadow-sm hover:shadow-md transition-all border-border/60">
      <CardHeader>
        <CardTitle className="flex items-center gap-3 text-lg">
          {icon}
          {title}
        </CardTitle>
      </CardHeader>
      <CardContent>
        <div className="text-4xl font-bold">{value}</div>
        <div className="text-sm text-muted-foreground mt-1">{trend}</div>
      </CardContent>
    </Card>
  );
}

function QuickActions() {
  return (
    <Card className="shadow-sm border-border/60">
      <CardHeader>
        <CardTitle className="text-lg">Actions rapides</CardTitle>
      </CardHeader>
      <CardContent className="flex gap-4 flex-wrap">
        <ActionButton label="Créer un board" />
        <ActionButton label="Ajouter une tâche" />
        <ActionButton label="Inviter un collaborateur" />
        <ActionButton label="Voir mes tâches" />
      </CardContent>
    </Card>
  );
}

function ActionButton({ label }: ActionButtonProps) {
  return (
    <button className="px-4 py-2 rounded-md bg-primary/10 text-primary hover:bg-primary/20 transition-colors text-sm font-medium">
      {label}
    </button>
  );
}

function SpotlightBoards() {
  return (
    <Card className="shadow-sm border-border/60">
      <CardHeader>
        <CardTitle className="text-lg">Boards en vedette</CardTitle>
      </CardHeader>
      <CardContent className="grid gap-4 md:grid-cols-2">
        <SpotlightBoard title="Roadmap 2024" tasks={32} />
        <SpotlightBoard title="Marketing Sprint" tasks={18} />
        <SpotlightBoard title="Refonte UI" tasks={12} />
        <SpotlightBoard title="Support & Bugs" tasks={25} />
      </CardContent>
    </Card>
  );
}

function SpotlightBoard({ title, tasks }: SpotlightBoardProps) {
  return (
    <div className="p-4 rounded-lg border bg-card hover:bg-muted/40 transition-colors cursor-pointer">
      <div className="font-semibold">{title}</div>
      <div className="text-sm text-muted-foreground">{tasks} tâches</div>
    </div>
  );
}

function ActivityTimeline() {
  return (
    <Card className="shadow-sm border-border/60">
      <CardHeader>
        <CardTitle className="text-lg">Activité récente</CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        <TimelineItem text="Nouvelle tâche ajoutée : 'Refactor Auth'" time="Il y a 2h" />
        <TimelineItem text="Board 'Roadmap 2024' mis à jour" time="Il y a 5h" />
        <TimelineItem text="Collaborateur ajouté : Marie" time="Hier" />
        <TimelineItem text="Sprint 'Juillet' clôturé" time="Il y a 3 jours" />
      </CardContent>
    </Card>
  );
}

function TimelineItem({ text, time }: TimelineItemProps) {
  return (
    <div className="flex items-center gap-3">
      <Clock className="h-4 w-4 text-primary" />
      <div>
        <div className="font-medium">{text}</div>
        <div className="text-xs text-muted-foreground">{time}</div>
      </div>
    </div>
  );
}

function ProductivitySummary() {
  return (
    <Card className="shadow-sm border-border/60">
      <CardHeader>
        <CardTitle className="text-lg">Résumé de productivité</CardTitle>
      </CardHeader>
      <CardContent className="text-muted-foreground leading-relaxed space-y-2">
        <p>
          Vous avez complété <strong>42 tâches</strong> cette semaine, avec une
          augmentation de <strong>18%</strong> par rapport à la semaine dernière.
        </p>
        <p>
          Votre équipe reste très active, avec une moyenne de <strong>5.2 tâches</strong> par collaborateur.
        </p>
        <p>
          Les boards les plus actifs sont <strong>Roadmap 2024</strong> et <strong>Support & Bugs</strong>,
          ce qui montre une bonne dynamique entre vision long terme et opérationnel.
        </p>
      </CardContent>
    </Card>
  );
}
