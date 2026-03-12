import { useMemo } from "react";
import {
    LineChart,
    Line,
    XAxis,
    YAxis,
    Tooltip,
    ResponsiveContainer,
    CartesianGrid,
} from "recharts";
import { useSseTelemetry } from "../hooks/useSseTelemetry";

type ChartsPanelProps = {
    selectedTurbineId: string;
};

const ChartsPanel = ({ selectedTurbineId }: ChartsPanelProps) => {
    const minutesBack = 60;
    const { series, latest, debug } = useSseTelemetry(selectedTurbineId, minutesBack, 1000);

    const chartData = useMemo(() => {
        return series.map((p) => ({
            ...p,
            t: new Date(p.timestamp).toLocaleTimeString([], {
                hour: "2-digit",
                minute: "2-digit",
                second: "2-digit",
            }),
        }));
    }, [series]);

    const ChartCard = ({
                           title,
                           dataKey,
                           unit,
                       }: {
        title: string;
        dataKey: keyof typeof chartData[number];
        unit?: string;
    }) => (
        <div className="bg-slate-700 p-4 rounded-lg h-72 min-w-0 overflow-hidden">
            <div className="text-sm text-slate-200 mb-2">{title}</div>
            <div className="h-60 w-full min-w-0">
                <ResponsiveContainer width="100%" height="100%" minWidth={0}>
                    <LineChart data={chartData}>
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="t" tick={{ fontSize: 10 }} interval="preserveStartEnd" />
                        <YAxis tick={{ fontSize: 10 }} />
                        <Tooltip />
                        <Line
                            type="monotone"
                            stroke="#22c55e"
                            strokeWidth={2}
                            dataKey={dataKey as string}
                            dot={false}
                            isAnimationActive={false}
                        />
                    </LineChart>
                </ResponsiveContainer>
            </div>
            {unit && latest ? (
                <div className="text-xs text-slate-300 mt-2">
                    current: {(latest[dataKey as keyof typeof latest] as number).toFixed(2)} {unit}
                </div>
            ) : null}
        </div>
    );

    return (
        <div className="bg-slate-800 border border-slate-800 rounded-xl p-6">
            <h2 className="text-sm mb-4 text-slate-400 uppercase">Telemetry</h2>

            <div className="text-xs text-slate-400 mb-4">
                turbine={selectedTurbineId} | points={series.length} | window={minutesBack}m
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <ChartCard title="Power Output" dataKey="powerOutput" unit="kW" />
                <ChartCard title="Wind Speed" dataKey="windSpeed" unit="m/s" />
                <ChartCard title="Ambient Temperature" dataKey="ambientTemperature" unit="°C" />
                <ChartCard title="Vibration" dataKey="vibration" />
            </div>

            <div className="bg-slate-700 p-4 rounded-lg mt-6">
                <div className="text-xs text-slate-200">DEBUG: {debug}</div>
            </div>

            <div className="grid grid-cols-2 md:grid-cols-3 gap-3 mt-6">
                <div className="bg-slate-700 rounded-lg p-3">
                    <div className="text-xs text-slate-400">Wind Direction</div>
                    <div className="text-lg font-semibold">{latest?.windDirection?.toFixed(1)}°</div>
                </div>

                <div className="bg-slate-700 rounded-lg p-3">
                    <div className="text-xs text-slate-400">Rotor Speed</div>
                    <div className="text-lg font-semibold">{latest?.rotorSpeed?.toFixed(1)}</div>
                </div>

                <div className="bg-slate-700 rounded-lg p-3">
                    <div className="text-xs text-slate-400">Nacelle Direction</div>
                    <div className="text-lg font-semibold">{latest?.nacelleDirection?.toFixed(1)}°</div>
                </div>

                <div className="bg-slate-700 rounded-lg p-3">
                    <div className="text-xs text-slate-400">Blade Pitch</div>
                    <div className="text-lg font-semibold">{latest?.bladePitch?.toFixed(1)}°</div>
                </div>

                <div className="bg-slate-700 rounded-lg p-3">
                    <div className="text-xs text-slate-400">Generator Temp</div>
                    <div className="text-lg font-semibold">{latest?.generatorTemp?.toFixed(1)} °C</div>
                </div>

                <div className="bg-slate-700 rounded-lg p-3">
                    <div className="text-xs text-slate-400">Gearbox Temp</div>
                    <div className="text-lg font-semibold">{latest?.gearboxTemp?.toFixed(1)} °C</div>
                </div>
            </div>
        </div>
    );
};

export default ChartsPanel;