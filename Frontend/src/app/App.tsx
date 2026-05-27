import Header from "./Header";
import BoardList from "../features/boards/BoardList";
import CreateBoardForm from "../features/boards/CreateBoardForm";

function App() {
  return (
    <div>
      <Header />

      <div style={{ padding: 20 }}>
        <h1>TaskBoard Frontend</h1>

        <CreateBoardForm />
        <BoardList />
      </div>
    </div>
  );
}

export default App;
