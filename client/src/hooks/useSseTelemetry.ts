import { useEffect, useMemo, useState } from "react";
import { useRealtime } from "../context/RealtimeContext";

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

type UseSseTelemetryResult = {
    series: Telemetry[];
    latest: Telemetry | null;
    debug: string;
};

export const useSseTelemetry = (
    selectedTurbineId: string,
    minutesBack = 60,
    maxPoints = 1000
): UseSseTelemetryResult => {
    const apiUrl = import.meta.env.VITE_API_URL;
    const { eventSource, connectionId, debug: connectionDebug } = useRealtime();

    const [series, setSeries] = useState<Telemetry[]>([]);
    const [debug, setDebug] = useState("waiting for SSE connection...");

    useEffect(() => {
        if (!eventSource || !connectionId || !selectedTurbineId) return;

        let cancelled = false;
        let currentGroupName: string | null = null;
        let currentGroupHandler: ((event: MessageEvent) => void) | null = null;

        const subscribe = async () => {
            try {
                setDebug(`subscribing ${selectedTurbineId}...`);

                const subscribeUrl =
                    `${apiUrl}/api/realtime/telemetry` +
                    `?connectionId=${encodeURIComponent(connectionId)}` +
                    `&turbineId=${encodeURIComponent(selectedTurbineId)}` +
                    `&minutesBack=${minutesBack}` +
                    `&maxPoints=${maxPoints}`;

                const res = await fetch(subscribeUrl);

                if (!res.ok) {
                    setDebug(`subscribe failed: ${res.status}`);
                    return;
                }

                const response =
                    (await res.json()) as RealtimeListenResponse<Telemetry[]>;

                if (cancelled) return;

                const initial = [...response.data].sort(
                    (a, b) =>
                        new Date(a.timestamp).getTime() -
                        new Date(b.timestamp).getTime()
                );

                setSeries(initial);
                setDebug(`subscribed to ${response.group} | points=${initial.length}`);

                currentGroupName = response.group;

                const onGroupData = (event: MessageEvent) => {
                    if (cancelled) return;

                    try {
                        const parsed = JSON.parse(event.data);
                        const incoming: Telemetry[] = Array.isArray(parsed)
                            ? parsed
                            : [parsed];

                        const normalized = incoming
                            .filter((x) => x.turbineId === selectedTurbineId)
                            .sort(
                                (a, b) =>
                                    new Date(a.timestamp).getTime() -
                                    new Date(b.timestamp).getTime()
                            );

                        setSeries(normalized);
                        setDebug(
                            `push update | points=${normalized.length} | ${new Date().toLocaleTimeString()}`
                        );
                    } catch (err) {
                        console.error("group event parse error", err);
                        setDebug(`group event parse error: ${String(err)}`);
                    }
                };

                currentGroupHandler = onGroupData;
                eventSource.addEventListener(response.group, onGroupData);
            } catch (err) {
                console.error("subscribe error", err);
                setDebug(`subscribe error: ${String(err)}`);
            }
        };

        subscribe();

        return () => {
            cancelled = true;

            if (currentGroupName && currentGroupHandler) {
                eventSource.removeEventListener(currentGroupName, currentGroupHandler);
            }
        };
    }, [apiUrl, eventSource, connectionId, selectedTurbineId, minutesBack, maxPoints]);

    const latest = useMemo(() => {
        return series.length ? series[series.length - 1] : null;
    }, [series]);

    return {
        series,
        latest,
        debug: `${connectionDebug} | ${debug}`,
    };
};