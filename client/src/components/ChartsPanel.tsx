import { useEffect, useMemo, useState } from "react";

type Telemetry = {
    turbineId: string;
    turbineName: string;
    farmId: string;
    timestamp: string;
    windSpeed: number;
    windDirection: number;
    ambientTemperature: number;
    rotorSpeed: number;
    powerOutput: number;
    nacelleDirection: number;
    bladePitch: number;
    generatorTemp: number;
    gearboxTemp: number;
    vibration: number;
    status: string;
};

const ChartsPanel = () => {
    const selectedTurbineId = "turbine-alpha";
    const [series, setSeries] = useState<Telemetry[]>([]);
    const [debug, setDebug] = useState("not started");

    useEffect(() => {
        let alive = true;

        const load = async () => {
            try {
                const r = await fetch("/api/telemetry/latest");
                if (!r.ok) {
                    setDebug(`status ${r.status}`);
                    return;
                }

                const all = (await r.json()) as Telemetry[];
                const latest = all.find((x) => x.turbineId === selectedTurbineId);
                if (!latest) {
                    setDebug(`ok, but no data for ${selectedTurbineId}`);
                    return;
                }

                setDebug(`ok | ${latest.timestamp} | vib=${latest.vibration.toFixed(2)}`);

                if (!alive) return;

                setSeries((prev) => {
                    const next = [...prev, latest];
                    return next.length > 120 ? next.slice(next.length - 120) : next;
                });
            } catch (e) {
                setDebug(`ERROR: ${String(e)}`);
            }
        };

        load();
        const id = setInterval(load, 2000);
        return () => {
            alive = false;
            clearInterval(id);
        };
    }, [selectedTurbineId]);

    const latest = useMemo(() => {
        for (let i = series.length - 1; i >= 0; i--) {
            if (series[i].turbineId === selectedTurbineId) return series[i];
        }
        return null;
    }, [series, selectedTurbineId]);

    return (
        <div className="bg-slate-800 border border-slate-800 rounded-xl p-6">
            <h2 className="text-sm mb-4 text-slate-400 uppercase">Telemetry</h2>

            <div className="text-xs text-slate-400 mb-4">
                turbine={selectedTurbineId} | received={series.length}
            </div>

            <div className="grid grid-cols-2 gap-6">
                <div className="bg-slate-700 p-4 rounded-lg h-48">
                    Power Output: {latest ? latest.powerOutput.toFixed(2) : "-"}
                </div>
                <div className="bg-slate-700 p-4 rounded-lg h-48">
                    Wind Speed: {latest ? latest.windSpeed.toFixed(2) : "-"}
                </div>
                <div className="bg-slate-700 p-4 rounded-lg h-48">
                    Temperature:{" "}
                    {latest
                        ? `${latest.ambientTemperature.toFixed(1)} / ${latest.generatorTemp.toFixed(
                            1
                        )} / ${latest.gearboxTemp.toFixed(1)}`
                        : "-"}
                </div>
                <div className="bg-slate-700 p-4 rounded-lg h-48">
                    Vibration: {latest ? latest.vibration.toFixed(2) : "-"}
                </div>
            </div>

            <div className="bg-slate-700 p-4 rounded-lg mt-6">
                <div className="text-xs text-slate-200">DEBUG: {debug}</div>
            </div>
        </div>
    );
};

export default ChartsPanel;