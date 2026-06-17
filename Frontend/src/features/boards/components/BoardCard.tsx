import { useState } from "react";
import { EllipsisVerticalIcon } from "@heroicons/react/24/outline";
import type { BoardDto } from "../types/BoardTypes";

interface BoardCardProps {
  board: BoardDto;
  onEdit: (board: BoardDto) => void;
  onDelete: (board: BoardDto) => void;
  onOpen: (board: BoardDto) => void;
}

export function BoardCard({ board, onEdit, onDelete, onOpen }: BoardCardProps) {
  const [menuOpen, setMenuOpen] = useState(false);

  return (
    <div
      className="relative bg-white dark:bg-gray-800 rounded-lg shadow hover:shadow-lg transition cursor-pointer p-4 border border-gray-200 dark:border-gray-700"
      onClick={() => onOpen(board)}
    >
      {/* Title */}
      <h3 className="text-lg font-semibold text-gray-900 dark:text-gray-100">
        {board.name}
      </h3>

      {/* Description */}
      {board.description && (
        <p className="text-sm text-gray-500 dark:text-gray-400 mt-1 line-clamp-2">
          {board.description}
        </p>
      )}

      {/* Menu button */}
      <button
        className="absolute top-3 right-3 p-1 rounded hover:bg-gray-100 dark:hover:bg-gray-700"
        onClick={(e) => {
          e.stopPropagation();
          setMenuOpen((prev) => !prev);
        }}
      >
        <EllipsisVerticalIcon className="w-5 h-5 text-gray-600 dark:text-gray-300" />
      </button>

      {/* Dropdown menu */}
      {menuOpen && (
        <div
          className="absolute top-10 right-3 bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded shadow-lg z-20"
          onClick={(e) => e.stopPropagation()}
        >
          <button
            className="block w-full text-left px-4 py-2 text-sm hover:bg-gray-100 dark:hover:bg-gray-700"
            onClick={() => {
              setMenuOpen(false);
              onEdit(board);
            }}
          >
            Modifier
          </button>

          <button
            className="block w-full text-left px-4 py-2 text-sm text-red-600 hover:bg-red-50 dark:hover:bg-red-900/30"
            onClick={() => {
              setMenuOpen(false);
              onDelete(board);
            }}
          >
            Supprimer
          </button>
        </div>
      )}
    </div>
  );
}
