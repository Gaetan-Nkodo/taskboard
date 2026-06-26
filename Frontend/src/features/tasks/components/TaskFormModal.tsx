import { Dialog } from "@headlessui/react";
import { useState, useEffect } from "react";
import type { TaskDto } from "../types/TaskTypes";
import type { ColumnDto } from "@/features/boards/types/BoardTypes";

interface TaskFormModalProps {
  open: boolean;
  title: string;
  initialValues?: TaskDto;
  columns: ColumnDto[];
  onSubmit: (values: {
    name: string;
    description?: string | null;
    icon?: string | null;
    columnId: string;
  }) => Promise<void>;
  onClose: () => void;
}

export function TaskFormModal({
  open,
  title,
  initialValues,
  columns,
  onSubmit,
  onClose,
}: TaskFormModalProps) {
  const [name, setName] = useState(initialValues?.name ?? "");
  const [description, setDescription] = useState(initialValues?.description ?? "");
  const [icon, setIcon] = useState(initialValues?.icon ?? "");
  const [columnId, setColumnId] = useState(
    initialValues?.columnId ?? columns[0]?.id ?? ""
  );

  // Sync when initialValues change (important en mode édition)
  useEffect(() => {
    if (initialValues) {
      setName(initialValues.name);
      setDescription(initialValues.description ?? "");
      setIcon(initialValues.icon ?? "");
      setColumnId(initialValues.columnId);
    }
  }, [initialValues]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    await onSubmit({
      name,
      description: description || null,
      icon: icon || null,
      columnId,
    });

    onClose();
  };

  return (
    <Dialog open={open} onClose={onClose} className="relative z-50">
      <div className="fixed inset-0 bg-black/30" aria-hidden="true" />

      <div className="fixed inset-0 flex items-center justify-center p-4">
        <Dialog.Panel className="bg-white dark:bg-gray-800 rounded-lg p-6 w-full max-w-md shadow-xl">
          <Dialog.Title className="text-lg font-semibold mb-4 text-gray-900 dark:text-gray-100">
            {title}
          </Dialog.Title>

          <form onSubmit={handleSubmit} className="space-y-4">
            {/* Nom */}
            <div>
              <label className="block text-sm font-medium mb-1" htmlFor="name">
                Nom
              </label>
              <input
                type="text"
                className="w-full border rounded px-3 py-2 dark:bg-gray-700 dark:text-gray-100"
                id="name"
                value={name}
                onChange={(e) => setName(e.target.value)}
                required
              />
            </div>

            {/* Description */}
            <div>
              <label className="block text-sm font-medium mb-1" htmlFor="description">
                Description
              </label>
              <textarea
                className="w-full border rounded px-3 py-2 dark:bg-gray-700 dark:text-gray-100"
                rows={3}
                value={description ?? ""}
                onChange={(e) => setDescription(e.target.value)}
              />
            </div>

            {/* Icône */}
            <div>
              <label className="block text-sm font-medium mb-1">Icône (emoji)</label>
              <input
                type="text"
                className="w-full border rounded px-3 py-2 dark:bg-gray-700 dark:text-gray-100"
                value={icon ?? ""}
                onChange={(e) => setIcon(e.target.value)}
                placeholder="🔥 📌 🚀"
              />
            </div>

            {/* Colonne */}
            <div>
              <label className="block text-sm font-medium mb-1">Colonne</label>
              <select
                className="w-full border rounded px-3 py-2 dark:bg-gray-700 dark:text-gray-100"
                value={columnId}
                onChange={(e) => setColumnId(e.target.value)}
              >
                {columns.map((col) => (
                  <option key={col.id} value={col.id}>
                    {col.name}
                  </option>
                ))}
              </select>
            </div>

            {/* Actions */}
            <div className="flex justify-end gap-2 pt-4">
              <button
                type="button"
                onClick={onClose}
                className="px-4 py-2 text-sm rounded border dark:border-gray-600"
              >
                Annuler
              </button>

              <button
                type="submit"
                className="px-4 py-2 text-sm rounded bg-blue-600 text-white hover:bg-blue-700"
              >
                Enregistrer
              </button>
            </div>
          </form>
        </Dialog.Panel>
      </div>
    </Dialog>
  );
}
