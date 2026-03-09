import { useEffect, useMemo, useState } from "react";
import {
    LineChart,
    Line,
    XAxis,
    YAxis,
    Tooltip,
    ResponsiveContainer,
    CartesianGrid,
} from "recharts";

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

type RealtimeListenResponse<T> = {
    group: string;
    data: T;
};

const ChartsPanel = () => {
    const selectedTurbineId = "turbine-alpha";
    const minutesBack = 60;

    const [series, setSeries] = useState<Telemetry[]>([]);
    const [debug, setDebug] = useState("not started");

    useEffect(() => {
        let cancelled = false;
        let eventSource: EventSource | null = null;
        let removeGroupListener: (() => void) | null = null;

        const start = async () => {
            try {
                eventSource = new EventSource("/api/realtime/sse");

                eventSource.onopen = () => {
                    console.log("SSE OPEN");
                    setDebug("SSE open");
                };

                eventSource.addEventListener("message", (event) => {
                    console.log("DEFAULT MESSAGE EVENT", event.data);
                });

                const onConnected = async (event: MessageEvent) => {
                    if (cancelled) return;

                    const connectedPayload = JSON.parse(event.data);
                    const connectionId = connectedPayload.connectionId;

                    console.log("CONNECTED EVENT", connectedPayload);
                    console.log("PARSED CONNECTION ID", connectionId);
                    
                    setDebug(`connected | connectionId=${connectionId}`);

                    const url =
                        `/api/realtime/telemetry?connectionId=${encodeURIComponent(connectionId)}` +
                        `&turbineId=${encodeURIComponent(selectedTurbineId)}` +
                        `&minutesBack=${minutesBack}` +
                        `&maxPoints=1000`;

                    const r = await fetch(url);
                    if (!r.ok) {
                        setDebug(`subscribe failed: status ${r.status}`);
                        return;
                    }

                    const response = (await r.json()) as RealtimeListenResponse<Telemetry[]>;
                    console.log("SUBSCRIBE RESPONSE", response);
                    if (cancelled) return;

                    setSeries(
                        [...response.data].sort(
                            (a, b) =>
                                new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime()
                        )
                    );

                    setDebug(`subscribed to ${response.group} | points=${response.data.length}`);

                    const onGroupData = (pushEvent: MessageEvent) => {
                        console.log("GROUP EVENT RECEIVED", response.group, pushEvent.data);

                        const parsed = JSON.parse(pushEvent.data);
                        const incoming: Telemetry[] = Array.isArray(parsed) ? parsed : [parsed];

                        const normalized = incoming
                            .filter((p) => p.turbineId === selectedTurbineId)
                            .sort(
                                (a, b) =>
                                    new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime()
                            );

                        setSeries(normalized);
                        setDebug(`push update | points=${normalized.length} | ${new Date().toLocaleTimeString()}`);
                    };
                    eventSource?.addEventListener(response.group, onGroupData);

                    removeGroupListener = () => {
                        eventSource?.removeEventListener(response.group, onGroupData);
                    };
                };

                eventSource.addEventListener("connected", onConnected);

                eventSource.onerror = () => {
                    console.error("SSE ERROR");
                    setDebug("SSE connection error");
                };
            } catch (e) {
                setDebug(`SSE ERROR: ${String(e)}`);
            }
        };

        start();

        return () => {
            cancelled = true;
            removeGroupListener?.();
            eventSource?.close();
        };
    }, [selectedTurbineId, minutesBack]);

    const latest = useMemo(() => {
        return series.length ? series[series.length - 1] : null;
    }, [series]);

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
        <div className="bg-slate-700 p-4 rounded-lg h-64 min-w-0">
            <div className="text-sm text-slate-200 mb-2">{title}</div>
            <div className="h-52 w-full min-w-0">
                <ResponsiveContainer width="100%" height="100%">
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
                    current: {(latest[dataKey as keyof Telemetry] as number).toFixed(2)} {unit}
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
                <ChartCard title="Vibration" dataKey="vibration" unit="" />
            </div>

            <div className="bg-slate-700 p-4 rounded-lg mt-6">
                <div className="text-xs text-slate-200">DEBUG: {debug}</div>
            </div>
        </div>
    );
};

export default ChartsPanel;