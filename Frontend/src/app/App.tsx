import Header from "./Header";
import { Outlet } from "react-router-dom";

function App() {
  return (
    <div>
      <Header />

      <div style={{ padding: 20 }}>
        <h1>TaskBoard Frontend</h1>

        <Outlet />
      </div>
    </div>
  );
}

export default App;
