import { useAuth } from "../context/AuthContext";
import TurbineList from "../components/TurbineList";
import ChartsPanel from "../components/ChartsPanel";
import ControlsPanel from "../components/ControlsPanel";
import AlertsPanel from "../components/AlertsPanel";

const Dashboard = () => {
    const { logout, user } = useAuth();

    return (
        <div className="min-h-screen bg-slate-900 text-slate-200">
            <header className="flex justify-between items-center px-6 py-4 border-b border-slate-800 bg-slate-800">
                <h1 className="text-lg font-bold text-blue-400">
                    Offshore Wind Farm
                </h1>

                <div className="flex items-center gap-4">
                    <span className="text-sm text-slate-400">{user?.username}</span>
                    |
                    <button
                        onClick={logout}
                        className="text-sm text-slate-400 hover:text-red-300"
                    >
                        Logout
                    </button>
                </div>
            </header>

            <div className="grid grid-cols-12 gap-6 p-6">
                <div className="col-span-3">
                    <TurbineList />
                </div>

                <div className="col-span-6">
                    <ChartsPanel />
                </div>

                <div className="col-span-3 space-y-6">
                    <ControlsPanel />
                    <AlertsPanel />
                </div>
            </div>
        </div>
    );
};

export default Dashboard;