import type { BoardDto } from "../types/BoardTypes";

interface DeleteBoardModalProps {
  board: BoardDto | null;
  loading?: boolean;
  onConfirm: (id: string) => Promise<void>;
  onCancel: () => void;
}

export function DeleteBoardModal({
  board,
  loading = false,
  onConfirm,
  onCancel,
}: DeleteBoardModalProps) {
  if (!board) return null;

  return (
    <div className="fixed inset-0 bg-black/40 backdrop-blur-sm flex items-center justify-center z-50">
      <div className="bg-white dark:bg-gray-800 rounded-lg shadow-xl p-6 w-full max-w-md border border-gray-200 dark:border-gray-700">
        
        {/* Title */}
        <h2 className="text-lg font-semibold text-gray-900 dark:text-gray-100">
          Supprimer le board
        </h2>

        {/* Message */}
        <p className="text-sm text-gray-600 dark:text-gray-300 mt-2">
          Es-tu sûr de vouloir supprimer le board{" "}
          <span className="font-medium text-gray-900 dark:text-gray-100">
            {board.name}
          </span>
          ? Cette action est irréversible.
        </p>

        {/* Actions */}
        <div className="flex justify-end gap-3 mt-6">
          <button
            className="px-4 py-2 rounded-md text-sm bg-gray-200 dark:bg-gray-700 hover:bg-gray-300 dark:hover:bg-gray-600"
            onClick={onCancel}
            disabled={loading}
          >
            Annuler
          </button>

          <button
            className="px-4 py-2 rounded-md text-sm bg-red-600 text-white hover:bg-red-700 disabled:opacity-50"
            onClick={() => onConfirm(board.id)}
            disabled={loading}
          >
            {loading ? "Suppression..." : "Supprimer"}
          </button>
        </div>
      </div>
    </div>
  );
}
