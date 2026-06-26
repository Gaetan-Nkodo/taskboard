import { useState } from "react";

interface SimpleColumn {
  id: string;
  name: string;
}

interface Props {
  columns: SimpleColumn[];
  onSubmit: (values: {
    name: string;
    description: string;
    columnId: string;
  }) => void;
  onCancel: () => void;
}

export function TaskForm({ columns, onSubmit, onCancel }: Props) {
  const [name, setName] = useState("");
  const [columnId, setColumnId] = useState(columns[0]?.id ?? "");

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    onSubmit({
      name,
      description: "",
      columnId,
    });
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">

      <input
        type="text"
        placeholder="Nom de la tâche"
        value={name}
        onChange={(e) => setName(e.target.value)}
        className="border rounded px-3 py-2 w-full"
        required
      />

      <select
        value={columnId}
        onChange={(e) => setColumnId(e.target.value)}
        className="border rounded px-3 py-2 w-full"
      >
        {columns.map((col) => (
          <option key={col.id} value={col.id}>
            {col.name}
          </option>
        ))}
      </select>

      <div className="flex justify-end gap-2">
        <button
          type="button"
          onClick={onCancel}
          className="px-4 py-2 border rounded"
        >
          Annuler
        </button>

        <button
          type="submit"
          className="px-4 py-2 bg-blue-600 text-white rounded"
        >
          Enregistrer
        </button>
      </div>
    </form>
  );
}
