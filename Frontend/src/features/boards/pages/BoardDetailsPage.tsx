import { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";
import { useBoardService } from "@/features/boards/services/BoardService";
import { useTasks } from "@/features/tasks/hooks/useTasks";
import { TaskColumn } from "@/features/tasks/components/TaskColumn";
import { TaskFormModal } from "@/features/tasks/components/TaskFormModal";
import { TaskFiltersBar } from "@/features/tasks/components/TaskFiltersBar";
import type { BoardDto } from "@/features/boards/types/BoardTypes";
import type { TaskDto } from "@/features/tasks/types/TaskTypes";

import {
  DndContext,
  closestCenter,
  PointerSensor,
  useSensor,
  useSensors,
} from "@dnd-kit/core";

import { arrayMove } from "@dnd-kit/sortable";

export default function BoardDetailsPage() {
  const { id: boardId } = useParams<{ id: string }>();
  const boardService = useBoardService();

  const {
    tasks,
    loading: tasksLoading,
    createTask,
    updateTask,
    deleteTask,
    moveTask,
  } = useTasks(boardId);

  const [board, setBoard] = useState<BoardDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [localTasks, setLocalTasks] = useState<TaskDto[]>([]);
  const [openForm, setOpenForm] = useState({
    mode: "create" as "create" | "edit",
    columnId: null as string | null,
    task: null as TaskDto | null,
  });

  const [filters, setFilters] = useState({
    search: "",
    status: "all",
    assignee: "all",
    sort: "order",
  });

  const sortTasks = (a: TaskDto, b: TaskDto): number => {
    switch (filters.sort) {
      case "name":
        return a.name.localeCompare(b.name) || a.order - b.order;
      case "column": {
        const colA = board?.columns.find((c) => c.id === a.columnId)?.name ?? "";
        const colB = board?.columns.find((c) => c.id === b.columnId)?.name ?? "";
        return colA.localeCompare(colB) || a.order - b.order;
      }
      case "assignee":
        return (a.assigneeId ?? "").localeCompare(b.assigneeId ?? "") || a.order - b.order;
      default:
        return a.order - b.order;
    }
  };

  // Écoute de l’événement custom "dnd-move" (utilisé dans les tests)
  useEffect(() => {
    const handler = (e: any) => {
      if (!e.detail) return;

      const { taskId, toColumnId, toIndex } = e.detail;

      moveTask(taskId, { columnId: toColumnId, order: toIndex });
    };

    window.addEventListener("dnd-move", handler);
    return () => window.removeEventListener("dnd-move", handler);
  }, [moveTask]);

  const filteredTasks = localTasks
    .filter((t) => {
      if (filters.status !== "all" && t.columnId !== filters.status) return false;
      if (filters.search && !t.name.toLowerCase().includes(filters.search.toLowerCase()))
        return false;
      return true;
    })
    .sort(sortTasks);

  // Chargement du board
  useEffect(() => {
    if (!boardId) return;

    let cancelled = false;

    const load = async () => {
      try {
        setLoading(true);
        setError(null);

        const data = await boardService.getBoard(boardId);
        if (!cancelled) setBoard(data);

      } catch {
        if (!cancelled) setError("Erreur lors du chargement du board");
      } finally {
        if (!cancelled) setLoading(false);
      }
    };

    load();
    return () => { cancelled = true };
  }, [boardId, boardService]);

  // Sync des tâches locales avec le hook
  useEffect(() => {
    setLocalTasks(tasks);
  }, [tasks]);

  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: { distance: 8 },
    })
  );

  const handleDragEnd = async (event: any) => {
    const { active, over } = event;
    if (!over) return;

    const activeId = active.id;
    const overId = over.id;

    const activeTask = localTasks.find((t) => t.id === activeId);

    // Drop dans une colonne vide
    if (over.data?.current?.type === "column") {
      const newColumnId = over.data.current.columnId;

      setLocalTasks(localTasks.map(t =>
        t.id === activeId ? { ...t, columnId: newColumnId, order: 0 } : t
      ));

      await moveTask(activeId, { columnId: newColumnId, order: 0 });
      return;
    }

    const overTask = localTasks.find((t) => t.id === overId);
    if (!activeTask || !overTask) return;

    const sameColumn = activeTask.columnId === overTask.columnId;

    if (sameColumn) {
      const columnTasks = localTasks
        .filter((t) => t.columnId === activeTask.columnId)
        .sort((a, b) => a.order - b.order);

      const oldIndex = columnTasks.findIndex((t) => t.id === activeId);
      const newIndex = columnTasks.findIndex((t) => t.id === overId);

      const reordered = arrayMove(columnTasks, oldIndex, newIndex);

      const others = localTasks.filter((t) => t.columnId !== activeTask.columnId);
      const next = [...others, ...reordered];

      setLocalTasks(next);

      await moveTask(activeTask.id, {
        columnId: activeTask.columnId,
        order: newIndex,
      });
    } else {
      const next = localTasks.map((t) =>
        t.id === activeId ? { ...t, columnId: overTask.columnId, order: 0 } : t
      );

      setLocalTasks(next);

      await moveTask(activeTask.id, {
        columnId: overTask.columnId,
        order: 0,
      });
    }
  };

  const isLoading = loading || tasksLoading;

  return (
    <div className="p-6 max-w-7xl mx-auto">
      <Link to="/boards" className="text-sm text-blue-600 hover:underline inline-block mb-4">
        ← Retour aux boards
      </Link>

      {isLoading ? (
        <div className="h-8 w-48 bg-gray-200 dark:bg-gray-700 animate-pulse rounded mb-2" />
      ) : error ? (
        <div className="text-red-600 mb-4">{error}</div>
      ) : (
        <>
          <h1 className="text-2xl font-bold">{board?.name}</h1>

          {board?.description && (
            <p className="text-sm text-gray-600 dark:text-gray-300 mt-1 mb-6">
              {board.description}
            </p>
          )}

          <TaskFiltersBar
            search={filters.search}
            status={filters.status}
            assignee={filters.assignee}
            sort={filters.sort}
            assignees={[]}
            onSearchChange={(v) => setFilters({ ...filters, search: v })}
            onStatusChange={(v) => setFilters({ ...filters, status: v })}
            onAssigneeChange={(v) => setFilters({ ...filters, assignee: v })}
            onSortChange={(v) => setFilters({ ...filters, sort: v })}
            onReset={() =>
              setFilters({ search: "", status: "all", assignee: "all", sort: "order" })
            }
          />

          <DndContext sensors={sensors} collisionDetection={closestCenter} onDragEnd={handleDragEnd}>
            <div className="flex gap-4 overflow-x-auto pb-4">
              {board?.columns
                .slice()
                .sort((a, b) => a.order - b.order)
                .map((column) => {
                  const columnTasks = filteredTasks.filter((t) => t.columnId === column.id);

                  return (
                    <TaskColumn
                      key={column.id}
                      column={column}
                      tasks={columnTasks}
                      onCreate={() =>
                        setOpenForm({ mode: "create", columnId: column.id, task: null })
                      }
                      onEdit={(task) =>
                        setOpenForm({ mode: "edit", columnId: task.columnId, task })
                      }
                      onDelete={(task) => deleteTask(task.id)}
                      onChangeStatus={(task, newColumnId) =>
                        updateTask(task.id, {
                          name: task.name,
                          description: task.description,
                          icon: task.icon,
                          columnId: newColumnId,
                        })
                      }
                    />
                  );
                })}
            </div>
          </DndContext>
        </>
      )}

      {openForm.columnId && (
        <TaskFormModal
          open={true}
          title={openForm.mode === "create" ? "Créer une tâche" : "Modifier la tâche"}
          initialValues={openForm.task ?? undefined}
          columns={board?.columns ?? []}
          onSubmit={async (values) => {
            if (openForm.mode === "create") {
              await createTask({
                columnId: openForm.columnId!,
                name: values.name,
                description: values.description ?? null,
                icon: values.icon ?? null
              });
            } else {
              await updateTask(openForm.task!.id, {
                name: values.name,
                description: values.description ?? null,
                icon: values.icon ?? null,
                columnId: values.columnId
              });
            }

            setOpenForm({ mode: "create", columnId: null, task: null });
          }}
          onClose={() => setOpenForm({ mode: "create", columnId: null, task: null })}
        />
      )}
    </div>
  );
}
