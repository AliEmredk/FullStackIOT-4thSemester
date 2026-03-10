interface Props {
    selectedTurbineId: string;
}

const AlertsPanel = ({ selectedTurbineId }: Props) => {
    return (
        <div className="bg-slate-800 border border-slate-800 rounded-xl p-4">
            <h2 className="text-sm mb-4 text-slate-400 uppercase">
                Active Alerts
            </h2>

            <div className="border-l-1 border-yellow-500 bg-yellow-500/10 p-3 rounded mb-3">
                <p className="text-sm font-semibold">
                    {selectedTurbineId}:
                    Generator temperature elevated
                </p>
                <p className="text-xs text-slate-400">10:30</p>
            </div>
        </div>
    );
};

export default AlertsPanel;