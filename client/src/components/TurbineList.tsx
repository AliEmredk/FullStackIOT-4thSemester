type TurbineItem = {
    turbineId: string;
    turbineName: string;
    status: string;
};

type TurbineListProps = {
    turbines: TurbineItem[];
    selectedTurbineId: string;
    onSelectTurbine: (id: string) => void;
};

const TurbineList = ({
                         turbines,
                         selectedTurbineId,
                         onSelectTurbine,
                     }: TurbineListProps) => {
    return (
        <div className="bg-slate-800 border border-slate-800 rounded-xl p-4">
            <h2 className="text-sm mb-4 text-slate-400 uppercase">Turbines</h2>

            {turbines.map((t) => {
                const isSelected = t.turbineId === selectedTurbineId;

                return (
                    <button
                        key={t.turbineId}
                        onClick={() => onSelectTurbine(t.turbineId)}
                        className={`w-full text-left p-3 mb-2 rounded-lg transition ${
                            isSelected
                                ? "bg-blue-600 text-white"
                                : "bg-slate-700 hover:bg-slate-600"
                        }`}
                    >
                        <div className="flex items-center justify-between">
                            <span>{t.turbineName}</span>

                            <div className="flex items-center gap-2">
                                <span
                                    className={`w-3 h-3 rounded-full ${
                                        t.status === "running"
                                            ? "bg-green-500"
                                            : "bg-red-500"
                                    }`}
                                />
                                <span className="text-xs opacity-80 capitalize">
                                    {t.status}
                                </span>
                            </div>
                        </div>
                    </button>
                );
            })}
        </div>
    );
};

export default TurbineList;