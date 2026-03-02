import { useAuth } from "./context/AuthContext";
import Login from "./pages/Login";
import Dashboard from "./pages/Dashboard";

function App() {
    const { user } = useAuth();

    return user ? <Dashboard /> : <Login />;
}

export default App;