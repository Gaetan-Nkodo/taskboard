import { render } from "@testing-library/react";
import { TestProviders } from "./test-utils";

export function renderWithProviders(ui: React.ReactElement) {
  return render(<TestProviders>{ui}</TestProviders>);
}
