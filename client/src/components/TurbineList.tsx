interface Turbine {
    id: string;
    name: string;
}

interface Props {
    turbines: Turbine[];
    selectedTurbineId: string;
    onChangeTurbine: (id: string) => void;
}

const TurbineList = ({ turbines, selectedTurbineId, onChangeTurbine }: Props) => {
    return (
        <div className="bg-slate-800 border border-slate-800 rounded-xl p-4">
            <h2 className="text-sm mb-4 text-slate-400 uppercase">Turbines</h2>
            {turbines.map((t) => (
                <button 
                key={t.id}
                onClick={() => onChangeTurbine(t.id)}
                className={`block w-full text-left px-3 py-2 mb-2 rounded
                ${
                    selectedTurbineId === t.id
                    ? "bg-blue-500 text-white"
                        : "bg-slate-700 hover:bg-slate-600"
                }`}
                >
                    {t.name}
                </button>
            ))}
        </div>
    );
};

export default TurbineList;