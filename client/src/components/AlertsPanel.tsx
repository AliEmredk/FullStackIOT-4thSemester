import { useEffect, useState } from "react";

interface Props {
    selectedTurbineId: string;
}

interface Alert {
    message: string;
    severity: string;
    timestamp: string;
}

const AlertsPanel = ({ selectedTurbineId }: Props) => {
    const apiUrl = import.meta.env.VITE_API_URL;
    const [alerts, setAlerts] = useState<Alert[]>([]);

    useEffect(() => {
        if (!apiUrl || !selectedTurbineId) return;

        const controller = new AbortController();

        const loadAlerts = async () => {
            try {
                const res = await fetch(
                    `${apiUrl}/api/alerts?turbineId=${encodeURIComponent(selectedTurbineId)}`,
                    { signal: controller.signal }
                );

                if (!res.ok) {
                    console.error("Failed to load alerts:", res.status);
                    setAlerts([]);
                    return;
                }

                const data = await res.json();
                setAlerts(data);
            } catch (err) {
                if ((err as Error).name !== "AbortError") {
                    console.error("Alerts fetch error:", err);
                    setAlerts([]);
                }
            }
        };

        loadAlerts();

        return () => controller.abort();
    }, [apiUrl, selectedTurbineId]);

    const getSeverityStyles = (severity: string) => {
        if (severity.toLowerCase() === "critical") {
            return "border-red-500 bg-red-500/10";
        }

        if (severity.toLowerCase() === "warning") {
            return "border-yellow-500 bg-yellow-500/10";
        }

        return "border-slate-500 bg-slate-500/10";
    };

    return (
        <div className="bg-slate-800 border border-slate-800 rounded-xl p-4">
            <h2 className="text-sm mb-4 text-slate-400 uppercase">
                Active Alerts
            </h2>

            {alerts.length === 0 && (
                <p className="text-sm text-slate-500">No alerts</p>
            )}

            {alerts.map((alert, i) => (
                <div
                    key={i}
                    className={`border-l-4 p-3 rounded mb-3 ${getSeverityStyles(alert.severity)}`}
                >
                    <p className="text-sm font-semibold">
                        {alert.severity}: {alert.message}
                    </p>

                    <p className="text-xs text-slate-400">
                        {new Date(alert.timestamp).toLocaleString("en-GB")}
                    </p>
                </div>
            ))}
        </div>
    );
};

export default AlertsPanel;