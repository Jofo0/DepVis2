import { BrowserRouter as Router } from "react-router-dom";
import Routes from "./Routes";
import { ThemeProvider } from "./theme/ThemeProvider";
import { IntlProvider } from "react-intl";

export default function App() {
  return (
    <ThemeProvider>
      <IntlProvider locale="en">
        <Router>
          <Routes />
        </Router>
      </IntlProvider>
    </ThemeProvider>
  );
}
