import { render } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { TasksPage } from "../TasksPage";
import { TestProviders } from "@/tests/TestProviders";

describe("TasksPage", () => {
  it("renders page", () => {
    render(
      <MemoryRouter initialEntries={["/tasks"]}>
        <TestProviders>
          <Routes>
            <Route path="/tasks" element={<TasksPage />} />
          </Routes>
        </TestProviders>
      </MemoryRouter>
    );
  });
});
