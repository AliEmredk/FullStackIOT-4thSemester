const turbines = ["Alpha", "Beta", "Gamma"];

const TurbineList = () => {
    return (
        <div className="bg-slate-800 border border-slate-800 rounded-xl p-4">
            <h2 className="text-sm mb-4 text-slate-400 uppercase">Turbines</h2>

            {turbines.map((t) => (
                <div
                    key={t}
                    className="p-3 mb-2 rounded-lg bg-slate-700 hover:bg-slate-700 cursor-pointer transition"
                >
                    {t}
                </div>
            ))}
        </div>
    );
};

export default TurbineList;