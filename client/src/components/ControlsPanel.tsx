import { useState } from "react";

const ControlsPanel = () => {
    const [bladePitch, setBladePitch] = useState(15);

    return (
        <div className="bg-slate-800 border border-slate-800 rounded-xl p-4">
            <h2 className="text-sm mb-4 text-slate-400 uppercase">
                Turbine Controls
            </h2>

            <button className="w-full mb-3 bg-green-600 hover:bg-green-500 py-2 rounded-lg">
                Start Turbine
            </button>

            <button className="w-full mb-3 bg-yellow-600 hover:bg-yellow-500 py-2 rounded-lg">
                Stop Turbine
            </button>

            <button className="w-full bg-red-600 hover:bg-red-500 py-2 rounded-lg">
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
                        className="w-full h-2 bg-slate-700 rounded-lg appearance-none cursor-pointer accent-blue-500"
                    />
                </div>
            </div>
        </div>
    );
};

export default ControlsPanel;