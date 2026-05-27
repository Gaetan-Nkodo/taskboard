import { RouterProvider } from "react-router-dom";
import { router } from "./routes";

export const AppProviders = () => {
  return <RouterProvider router={router} />;
};
