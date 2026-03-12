import { useAuth } from "./context/AuthContext";
import Login from "./pages/Login";
import Dashboard from "./pages/Dashboard";
import { RealtimeProvider } from "./context/RealtimeContext";

function App() {
    const { user } = useAuth();

    if (!user) return <Login />;

    return (
        <RealtimeProvider>
            <Dashboard />
        </RealtimeProvider>
    );
}

export default App;