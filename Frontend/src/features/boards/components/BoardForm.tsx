import { useState } from "react";
import type { CreateBoardRequest, UpdateBoardRequest } from "../types/BoardTypes";

interface BoardFormProps {
  initialValues?: {
    name: string;
    description?: string | null;
  };
  loading?: boolean;
  onSubmit: (data: CreateBoardRequest | UpdateBoardRequest) => Promise<void>;
  onCancel: () => void;
}

export function BoardForm({
  initialValues,
  loading = false,
  onSubmit,
  onCancel,
}: BoardFormProps) {
  const [name, setName] = useState(initialValues?.name ?? "");
  const [description, setDescription] = useState(initialValues?.description ?? "");
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!name.trim()) {
      setError("Le nom du board est requis.");
      return;
    }

    setError(null);

    await onSubmit({
      name: name.trim(),
      description: description?.trim() || null,
    });
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4 p-2">
      {/* Error */}
      {error && (
        <div className="text-red-600 text-sm font-medium">{error}</div>
      )}

      {/* Name */}
      <div>
        <label htmlFor="board-name" className="block text-sm font-medium text-gray-700 dark:text-gray-300">
          Nom du board
        </label>
        <input
          type="text"
          id="board-name"
          className="mt-1 w-full rounded-md border border-gray-300 dark:border-gray-700 bg-white dark:bg-gray-800 px-3 py-2 text-sm focus:ring-2 focus:ring-blue-500 focus:outline-none"
          value={name}
          onChange={(e) => setName(e.target.value)}
          disabled={loading}
        />
      </div>

      {/* Description */}
      <div>
        <label htmlFor="board-description" className="block text-sm font-medium text-gray-700 dark:text-gray-300">
          Description
        </label>
        <textarea
          id="board-description"
          className="mt-1 w-full rounded-md border border-gray-300 dark:border-gray-700 bg-white dark:bg-gray-800 px-3 py-2 text-sm focus:ring-2 focus:ring-blue-500 focus:outline-none"
          rows={3}
          value={description ?? ""}
          onChange={(e) => setDescription(e.target.value)}
          disabled={loading}
        />
      </div>

      {/* Actions */}
      <div className="flex justify-end gap-3 pt-2">
        <button
          type="button"
          className="px-4 py-2 rounded-md text-sm bg-gray-200 dark:bg-gray-700 hover:bg-gray-300 dark:hover:bg-gray-600"
          onClick={onCancel}
          disabled={loading}
        >
          Annuler
        </button>

        <button
          type="submit"
          className="px-4 py-2 rounded-md text-sm bg-blue-600 text-white hover:bg-blue-700 disabled:opacity-50"
          disabled={loading}
        >
          {loading ? "Enregistrement..." : "Enregistrer"}
        </button>
      </div>
    </form>
  );
}
