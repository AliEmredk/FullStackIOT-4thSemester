import { useAuth } from "../context/AuthContext";
import TurbineList from "../components/TurbineList";
import ChartsPanel from "../components/ChartsPanel";
import ControlsPanel from "../components/ControlsPanel";
import AlertsPanel from "../components/AlertsPanel";
import {useState, useEffect} from "react";

interface Turbine {
    id: string;
    name: string;
    status: "running" | "stopped";
}

const Dashboard = () => {
    const { logout, user } = useAuth();
    
    const [turbineList, setTurbineList] = useState<Turbine[]>([]);
    const [selectedTurbineId, setSelectedTurbineId] = useState<string>("turbine-alpha");

    useEffect(() => {
        const loadTurbines = async () => {
            const res = await fetch("http://localhost:5096/api/telemetry/latest");
            const data = await res.json();

            const turbines = data.map((t: any) => ({
                id: t.turbineId,
                name: t.turbineName,
                status: t.status
            }));

            setTurbineList(turbines);
        };

        loadTurbines();
    }, []);

    useEffect(() => {
        const eventSource = new EventSource("http://localhost:5096/api/realtime/sse");

        eventSource.onopen = () => {
            console.log("SSE connected");
        };

        eventSource.onmessage = async (event) => {
            console.log("SSE RAW:", event.data);
            const msg = JSON.parse(event.data);
            console.log("SSE PARSED:", msg);

            // First message contains connectionId
            if (msg.connectionId) {
                const connectionId = msg.connectionId;

                for (const turbine of turbineList) {
                    await fetch(
                        `http://localhost:5096/api/realtime/telemetry?connectionId=${connectionId}&turbineId=${turbine.id}`
                    );
                }
            }

            // Actual telemetry updates
            if (msg.data) {
                const telemetryList = msg.data;

                setTurbineList(prev =>
                    prev.map(t => {
                        const update = telemetryList.find((x: any) => x.turbineId === t.id);

                        if (!update) return t;

                        return {
                            ...t,
                            status: update.status
                        };
                    })
                );
            }
        };

        return () => eventSource.close();
    }, [selectedTurbineId]);

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
                    <TurbineList 
                    turbines={turbineList}
                    selectedTurbineId={selectedTurbineId}
                    onChangeTurbine={setSelectedTurbineId}/>
                </div>

                <div className="col-span-6">
                    <ChartsPanel />
                </div>

                <div className="col-span-3 space-y-6">
                    <ControlsPanel 
                    turbines={turbineList}
                    setTurbines={setTurbineList}
                    selectedTurbineId={selectedTurbineId}
                    onChangeTurbine={setSelectedTurbineId}/>
                    <AlertsPanel selectedTurbineId={selectedTurbineId}/>
                </div>
            </div>
        </div>
    );
};

export default Dashboard;