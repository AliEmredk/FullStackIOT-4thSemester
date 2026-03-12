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
    const [alerts, setAlerts] = useState<Alert[]>([]);

    useEffect(() => {
        const loadAlerts = async () => {
            const res = await fetch(
                `http://localhost:5096/api/alerts?turbineId=${selectedTurbineId}`
            );

            const data = await res.json();
            setAlerts(data);
        };

        loadAlerts();
    }, [selectedTurbineId]);

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