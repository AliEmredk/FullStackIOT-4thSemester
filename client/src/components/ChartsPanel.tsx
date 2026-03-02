const ChartsPanel = () => {
    return (
        <div className="bg-slate-800 border border-slate-800 rounded-xl p-6">
            <h2 className="text-sm mb-4 text-slate-400 uppercase">
                Telemetry
            </h2>

            <div className="grid grid-cols-2 gap-6">
                <div className="bg-slate-700 p-4 rounded-lg h-48">
                    Power Output
                </div>
                <div className="bg-slate-700 p-4 rounded-lg h-48">
                    Wind Speed
                </div>
                <div className="bg-slate-700 p-4 rounded-lg h-48">
                    Temperature
                </div>
                <div className="bg-slate-700 p-4 rounded-lg h-48">
                    Vibration
                </div>
            </div>
        </div>
    );
};

export default ChartsPanel;