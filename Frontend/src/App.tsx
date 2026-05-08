import { BrowserRouter as Router } from "react-router-dom";
import Routes from "./Routes";
import { ThemeProvider } from "./theme/ThemeProvider";
import { IntlProvider } from "react-intl";
import { TooltipProvider } from "./components/ui/tooltip";

export default function App() {
  return (
    <ThemeProvider>
      <IntlProvider locale="en">
        <TooltipProvider>
          <Router>
            <Routes />
          </Router>
        </TooltipProvider>
      </IntlProvider>
    </ThemeProvider>
  );
}
