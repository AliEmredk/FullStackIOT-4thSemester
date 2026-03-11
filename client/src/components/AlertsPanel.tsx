type AlertsPanelProps = {
    selectedTurbineId: string;
};

const AlertsPanel = ({ selectedTurbineId }: AlertsPanelProps) => {
    return (
        <div className="bg-slate-800 border border-slate-800 rounded-xl p-4">
            <h2 className="text-sm mb-2 text-slate-400 uppercase">
                Active Alerts
            </h2>

            <p className="text-xs text-slate-500 mb-4">
                Selected turbine: {selectedTurbineId}
            </p>

            <div className="border-l-4 border-yellow-500 bg-yellow-500/10 p-3 rounded mb-3">
                <p className="text-sm font-semibold">
                    Generator temperature elevated
                </p>
                <p className="text-xs text-slate-400">10:30</p>
            </div>
        </div>
    );
};

export default AlertsPanel;