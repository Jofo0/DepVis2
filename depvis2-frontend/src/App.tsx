import Layout from "./pages/Layout";
import { BrowserRouter as Router } from "react-router-dom";
import Routes from "./Routes";

export default function App() {
  return (
    <Router>
      <Layout>
        <Routes />
      </Layout>
    </Router>
  );
}
