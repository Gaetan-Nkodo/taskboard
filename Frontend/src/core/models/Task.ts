export interface Task {
  id: string;
  title: string;
  description?: string;
  status: "Todo" | "InProgress" | "Done";
  boardId: string;
}
