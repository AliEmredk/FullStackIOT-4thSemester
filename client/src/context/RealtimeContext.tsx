import React, { createContext, useContext, useEffect, useMemo, useState } from "react";

type RealtimeContextType = {
    eventSource: EventSource | null;
    connectionId: string | null;
    debug: string;
};

const RealtimeContext = createContext<RealtimeContextType | null>(null);

export const RealtimeProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const apiUrl = import.meta.env.VITE_API_URL;

    const [eventSource, setEventSource] = useState<EventSource | null>(null);
    const [connectionId, setConnectionId] = useState<string | null>(null);
    const [debug, setDebug] = useState("not started");

    useEffect(() => {
        const es = new EventSource(`${apiUrl}/api/realtime/sse`);

        setEventSource(es);
        setDebug("opening SSE...");

        es.onopen = () => {
            console.log("SSE OPEN");
            setDebug("SSE open");
        };

        const onConnected = (event: MessageEvent) => {
            try {
                const payload = JSON.parse(event.data);
                const id = payload.connectionId;

                if (!id) {
                    setDebug("connected event missing connectionId");
                    return;
                }

                console.log("SSE CONNECTED", id);
                setConnectionId(id);
                setDebug(`connected | connectionId=${id}`);
            } catch (err) {
                console.error("connected parse error", err);
                setDebug(`connected parse error: ${String(err)}`);
            }
        };

        es.addEventListener("connected", onConnected);

        es.onerror = () => {
            console.error("SSE ERROR");
            setDebug("SSE error");
        };

        return () => {
            es.removeEventListener("connected", onConnected);
            es.close();
            setEventSource(null);
            setConnectionId(null);
        };
    }, [apiUrl]);

    const value = useMemo(
        () => ({
            eventSource,
            connectionId,
            debug,
        }),
        [eventSource, connectionId, debug]
    );

    return (
        <RealtimeContext.Provider value={value}>
            {children}
        </RealtimeContext.Provider>
    );
};

export const useRealtime = () => {
    const context = useContext(RealtimeContext);

    if (!context) {
        throw new Error("useRealtime must be used within RealtimeProvider");
    }

    return context;
};