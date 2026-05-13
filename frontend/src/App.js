import { useEffect, useState } from "react";

function App() {
  const [incidents, setIncidents] = useState([]);
  const [logs, setLogs] = useState([]);

  useEffect(() => {
    fetchLogs();
    fetchIncidents();

    const interval = setInterval(() => {
      fetchLogs();
      fetchIncidents();
    }, 3000);

    return () => clearInterval(interval);
  }, []);

  const fetchLogs = async () => {
    try {
      const res = await fetch("http://localhost:5000/api/logs");

      const data = await res.json();

      setLogs(data);
    } catch (err) {
      console.log(err);
    }
  };

  const fetchIncidents = async () => {
    try {
      const res = await fetch("http://localhost:5000/api/incidents");

      const data = await res.json();

      setIncidents(data);
    } catch (err) {
      console.log(err);
    }
  };

  return (
    <div style={styles.container}>

      {/* HEADER */}
      <div style={styles.header}>
        <h1 style={styles.title}>
          Incident Intelligence Dashboard
        </h1>

        <p style={styles.subtitle}>
          Real-time microservice monitoring system
        </p>
      </div>

      {/* GRID */}
      <div style={styles.grid}>

        {/* INCIDENT PANEL */}
        <div style={styles.panel}>

          <h2 style={styles.panelTitle}>
            Active Incidents
          </h2>

          {incidents.length === 0 ? (
            <p style={styles.empty}>
              No active incidents
            </p>
          ) : (
            incidents.map((i, index) => (

              <div
                key={index}
                style={styles.incidentCard}
              >

                {/* TOP */}
                <div style={styles.row}>
                  <span style={styles.service}>
                    {i.serviceName}
                  </span>

                  <span style={styles.severity}>
                    {i.severity}
                  </span>
                </div>

                {/* MESSAGE */}
                <p style={styles.message}>
                  {i.message}
                </p>

                {/* AI SECTION */}
                <div style={styles.aiBox}>

                  <div style={styles.aiTitle}>
                    AI Root Cause Analysis
                  </div>

                  <p style={styles.aiText}>
                    <strong>Root Cause:</strong>
                    {" "}
                    {i.rootCause || "N/A"}
                  </p>

                  <p style={styles.aiText}>
                    <strong>Recommendation:</strong>
                    {" "}
                    {i.recommendation || "N/A"}
                  </p>

                  <p style={styles.aiText}>
                    <strong>Priority:</strong>
                    {" "}
                    {i.priority || "N/A"}
                  </p>

                </div>

              </div>
            ))
          )}
        </div>

        {/* LOGS PANEL */}
        <div style={styles.panel}>

          <h2 style={styles.panelTitle}>
            System Logs
          </h2>

          {logs.length === 0 ? (
            <p style={styles.empty}>
              No logs available
            </p>
          ) : (
            logs.map((l, index) => (
              <div
                key={index}
                style={styles.logCard}
              >

                <span style={styles.logService}>
                  {l.serviceName}
                </span>

                <span style={styles.logMessage}>
                  {l.message}
                </span>

              </div>
            ))
          )}
        </div>

      </div>
    </div>
  );
}

const styles = {
  container: {
    fontFamily:
      "system-ui, -apple-system, Segoe UI, Roboto, Arial",

    backgroundColor: "#0f172a",

    minHeight: "100vh",

    padding: "20px",

    color: "#e5e7eb"
  },

  header: {
    marginBottom: "20px",

    borderBottom: "1px solid #1f2937",

    paddingBottom: "10px"
  },

  title: {
    margin: 0,

    fontSize: "24px",

    fontWeight: "700",

    color: "#f9fafb"
  },

  subtitle: {
    margin: "5px 0 0",

    fontSize: "13px",

    color: "#9ca3af"
  },

  grid: {
    display: "grid",

    gridTemplateColumns: "1fr 1fr",

    gap: "20px"
  },

  panel: {
    backgroundColor: "#111827",

    border: "1px solid #1f2937",

    borderRadius: "12px",

    padding: "15px",

    height: "80vh",

    overflowY: "auto"
  },

  panelTitle: {
    fontSize: "16px",

    marginBottom: "15px",

    color: "#f3f4f6",

    borderBottom: "1px solid #1f2937",

    paddingBottom: "8px"
  },

  incidentCard: {
    backgroundColor: "#0b1220",

    border: "1px solid #1f2937",

    padding: "12px",

    borderRadius: "10px",

    marginBottom: "15px"
  },

  row: {
    display: "flex",

    justifyContent: "space-between",

    marginBottom: "8px"
  },

  service: {
    fontWeight: "700",

    fontSize: "14px"
  },

  severity: {
    fontSize: "11px",

    padding: "4px 8px",

    borderRadius: "6px",

    backgroundColor: "#991b1b",

    color: "white"
  },

  message: {
    fontSize: "13px",

    color: "#d1d5db",

    marginBottom: "12px"
  },

  aiBox: {
    backgroundColor: "#111827",

    border: "1px solid #374151",

    borderRadius: "8px",

    padding: "10px"
  },

  aiTitle: {
    fontSize: "12px",

    fontWeight: "700",

    color: "#60a5fa",

    marginBottom: "8px"
  },

  aiText: {
    fontSize: "12px",

    color: "#d1d5db",

    marginBottom: "6px",

    lineHeight: "1.5"
  },

  logCard: {
    fontSize: "12px",

    padding: "10px",

    borderBottom: "1px solid #1f2937",

    display: "flex",

    justifyContent: "space-between"
  },

  logService: {
    color: "#93c5fd",

    fontWeight: "600"
  },

  logMessage: {
    color: "#d1d5db"
  },

  empty: {
    color: "#6b7280",

    fontSize: "13px"
  }
};

export default App;