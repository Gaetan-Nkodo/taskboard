export interface TaskFiltersBarProps {
  search: string;
  status: string;
  assignee: string;
  sort: string;

  assignees: { id: string; name: string }[];

  onSearchChange: (value: string) => void;
  onStatusChange: (value: string) => void;
  onAssigneeChange: (value: string) => void;
  onSortChange: (value: string) => void;
  onReset: () => void;
}

export function TaskFiltersBar({
  search,
  status,
  assignee,
  sort,
  assignees,
  onSearchChange,
  onStatusChange,
  onAssigneeChange,
  onSortChange,
  onReset,
}: TaskFiltersBarProps) {
  return (
    <div className="flex flex-wrap gap-3 items-center mb-4 p-3 bg-gray-50 dark:bg-gray-800 rounded-lg border">

      {/* Recherche */}
      <input
        type="text"
        value={search}
        onChange={(e) => onSearchChange(e.target.value)}
        placeholder="Rechercher une tâche..."
        className="border rounded px-3 py-2 bg-white dark:bg-gray-700"
      />

      {/* Statut */}
      <select
        value={status}
        onChange={(e) => onStatusChange(e.target.value)}
        className="border rounded px-3 py-2 bg-white dark:bg-gray-700"
      >
        <option value="all">Tous les statuts</option>
        <option value="ToDo">À faire</option>
        <option value="InProgress">En cours</option>
        <option value="Done">Terminé</option>
      </select>

      {/* Assigné */}
      <select
        value={assignee}
        onChange={(e) => onAssigneeChange(e.target.value)}
        className="border rounded px-3 py-2 bg-white dark:bg-gray-700"
      >
        <option value="all">Tous les assignés</option>
        {assignees.map((a) => (
          <option key={a.id} value={a.id}>
            {a.name}
          </option>
        ))}
      </select>

      {/* Tri */}
      <select
        value={sort}
        onChange={(e) => onSortChange(e.target.value)}
        className="border rounded px-3 py-2 bg-white dark:bg-gray-700"
      >
        <option value="order">Ordre</option>
        <option value="name">Nom</option>
        <option value="column">Colonne</option>
        <option value="assignee">Assigné</option>
      </select>

      {/* Reset */}
      <button
        onClick={onReset}
        className="px-3 py-2 text-sm border rounded hover:bg-gray-100 dark:hover:bg-gray-700"
      >
        Réinitialiser
      </button>
    </div>
  );
}
