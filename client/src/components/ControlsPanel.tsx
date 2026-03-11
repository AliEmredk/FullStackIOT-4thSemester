import {useState} from "react";

interface Turbine {
    id: string;
    name: string;
    status: "running" | "stopped";
}

interface ControlsPanelProps {
    turbines: Turbine[];
    setTurbines: React.Dispatch<React.SetStateAction<Turbine[]>>;
    selectedTurbineId?: string;
    onChangeTurbine: (id: string) => void;
}

const ControlsPanel = ({ turbines, setTurbines, selectedTurbineId, onChangeTurbine }: ControlsPanelProps) => {
    const [bladePitch, setBladePitch] = useState(15);
    const selectedTurbine = turbines.find(t => t.id === selectedTurbineId);
    const isRunning = selectedTurbine?.status === "running";
    const sendCommand = async (action: string, payload?: object) => {
        if(!selectedTurbineId) return;

        const token = localStorage.getItem("token");
        const body = payload ? { action, ...payload } : { action };

        await fetch(`http://localhost:5096/api/windmills/${selectedTurbineId}/command`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`,
            },
            body: JSON.stringify(body),
        });
        
        if (action === "stop" || action === "start") {
            setTurbines(prev =>
                prev.map(t =>
                    t.id === selectedTurbineId
                        ? { ...t, status: action === "stop" ? "stopped" : "running" }
                        : t
                )
            );
        }
    };

    return (
        <div className="bg-slate-800 border border-slate-800 rounded-xl p-4">
            <h2 className="text-sm mb-4 text-slate-400 uppercase">
                Turbine Controls
            </h2>
            <select 
            value={selectedTurbineId}
            onChange={(e) => onChangeTurbine(e.target.value)}
            className="w-full mb-4 bg-slate-700 p-2 rounded"
            >
                {turbines.map((t) => (
                    <option key={t.id} value={t.id}>
                        {t.name}
                    </option>
                ))}
            </select>

            <button disabled={isRunning} 
                    className={`w-full mb-3 py-2 rounded-lg
                    ${isRunning
                    ? "bg-slate-600 cursor-not-allowed"
                    : "bg-green-600 hover:bg-green-500"}
                    `}
                    onClick={() => sendCommand("start")}>
                Start Turbine
            </button>

            <button disabled={!isRunning}
                className={`w-full mb-3 py-2 rounded-lg
                ${!isRunning
                ? "bg-slate-600 cursor-not-allowed"
                : "bg-yellow-600 hover:bg-yellow-500"}
                `}
                    onClick={() => sendCommand("stop")}>
                Stop Turbine
            </button>

            <button className="w-full bg-red-600 hover:bg-red-500 py-2 rounded-lg"
                    onClick={() => sendCommand("stop", { reason: "Emergency" })}>
                Emergency Stop
            </button>


            <div className="space-y-4 py-4">
                <div>
                    <label className="block text-sm text-slate-400 mb-2">
                        Blade Pitch: {bladePitch}°
                    </label>

                    <input
                        type="range"
                        min="0"
                        max="30"
                        value={bladePitch}
                        onChange={(e) => setBladePitch(Number(e.target.value))}
                        onMouseUp={() => sendCommand("setPitch", { angle: bladePitch })}
                        className="slider w-full h-2 bg-slate-700 rounded-lg appearance-none cursor-pointer"
                    />
                </div>
            </div>
        </div>
    );
};

export default ControlsPanel;