import { useState } from "react";
import { useBoards } from "./useBoards";

export default function CreateBoardForm() {
  const { createBoard } = useBoards();
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!name.trim()) return;

    await createBoard({ name, description });
    setName("");
    setDescription("");
    alert("Board créé !");
  };

  return (
    <form onSubmit={submit} style={{ marginBottom: "20px" }}>
      <input
        value={name}
        onChange={e => setName(e.target.value)}
        placeholder="Nom du board"
        style={{ padding: "8px", marginRight: "8px" }}
      />
      <input
        value={description}
        onChange={e => setDescription(e.target.value)}
        placeholder="Description"
        style={{ padding: "8px", marginRight: "8px" }}
      />
      <button type="submit">Créer</button>
    </form>
  );
}
