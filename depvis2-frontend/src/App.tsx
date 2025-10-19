import { BrowserRouter as Router } from "react-router-dom";
import Routes from "./Routes";
import { ThemeProvider } from "./theme/ThemeProvider";

export default function App() {
  return (
    <ThemeProvider>
      <Router>
        <Routes />
      </Router>
    </ThemeProvider>
  );
}
