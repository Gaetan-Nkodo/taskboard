import { useState } from "react";
import { useBoards } from "../hooks/useBoards";
import { BoardCard } from "../components/BoardCard";
import { BoardForm } from "../components/BoardForm";
import { DeleteBoardModal } from "../components/DeleteBoardModal";
import type { BoardDto } from "../types/BoardTypes";

export function BoardsPage() {
  const {
    boards,
    loading,
    error,
    createBoard,
    updateBoard,
    deleteBoard,
  } = useBoards();

  const [showCreate, setShowCreate] = useState(false);
  const [boardToEdit, setBoardToEdit] = useState<BoardDto | null>(null);
  const [boardToDelete, setBoardToDelete] = useState<BoardDto | null>(null);

  // -----------------------------
  // HANDLERS
  // -----------------------------
  const handleOpenBoard = (board: BoardDto) => {
    // Navigation vers BoardDetailsPage
    window.location.href = `/boards/${board.id}`;
  };

  const handleCreate = async (data: { name: string; description?: string | null }) => {
    await createBoard(data);
    setShowCreate(false);
  };

  const handleUpdate = async (data: { name: string; description?: string | null }) => {
    if (!boardToEdit) return;
    await updateBoard(boardToEdit.id, data);
    setBoardToEdit(null);
  };

  const handleDelete = async (id: string) => {
    await deleteBoard(id);
    setBoardToDelete(null);
  };

  // -----------------------------
  // RENDER
  // -----------------------------
  return (
    <div className="p-6 max-w-6xl mx-auto">
      {/* Header */}
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">
          Vos Boards
        </h1>

        <button
          className="px-4 py-2 rounded-md bg-blue-600 text-white hover:bg-blue-700"
          onClick={() => setShowCreate(true)}
        >
          Nouveau board
        </button>
      </div>

      {/* Error */}
      {error && (
        <div className="text-red-600 mb-4">{error}</div>
      )}

      {/* Loading */}
      {loading && (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          {[1, 2, 3].map((i) => (
            <div
              key={i}
              className="h-28 bg-gray-200 dark:bg-gray-700 animate-pulse rounded-lg"
            />
          ))}
        </div>
      )}

      {/* Boards */}
      {!loading && boards.length > 0 && (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          {boards.map((board) => (
            <BoardCard
              key={board.id}
              board={board}
              onOpen={handleOpenBoard}
              onEdit={(b) => setBoardToEdit(b)}
              onDelete={(b) => setBoardToDelete(b)}
            />
          ))}
        </div>
      )}

      {/* Empty state */}
      {!loading && boards.length === 0 && (
        <div className="text-center text-gray-500 dark:text-gray-400 mt-10">
          Aucun board pour le moment.
        </div>
      )}

      {/* Create Modal */}
      {showCreate && (
        <div className="fixed inset-0 bg-black/40 backdrop-blur-sm flex items-center justify-center z-50">
          <div className="bg-white dark:bg-gray-800 rounded-lg shadow-xl p-6 w-full max-w-md">
            <h2 className="text-lg font-semibold mb-4 text-gray-900 dark:text-gray-100">
              Nouveau board
            </h2>

            <BoardForm
              onSubmit={handleCreate}
              onCancel={() => setShowCreate(false)}
            />
          </div>
        </div>
      )}

      {/* Edit Modal */}
      {boardToEdit && (
        <div className="fixed inset-0 bg-black/40 backdrop-blur-sm flex items-center justify-center z-50">
          <div className="bg-white dark:bg-gray-800 rounded-lg shadow-xl p-6 w-full max-w-md">
            <h2 className="text-lg font-semibold mb-4 text-gray-900 dark:text-gray-100">
              Modifier le board
            </h2>

            <BoardForm
              initialValues={{
                name: boardToEdit.name,
                description: boardToEdit.description,
              }}
              onSubmit={handleUpdate}
              onCancel={() => setBoardToEdit(null)}
            />
          </div>
        </div>
      )}

      {/* Delete Modal */}
      <DeleteBoardModal
        board={boardToDelete}
        onConfirm={handleDelete}
        onCancel={() => setBoardToDelete(null)}
      />
    </div>
  );
}
