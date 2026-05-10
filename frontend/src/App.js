import { useEffect, useState } from "react";

function App() {
  const [incidents, setIncidents] = useState([]);
  const [logs, setLogs] = useState([]);

  useEffect(() => {
    const interval = setInterval(() => {
      fetchLogs();
      fetchIncidents();
    }, 3000);

    fetchLogs();
    fetchIncidents();

    return () => clearInterval(interval);
  }, []);

  const fetchLogs = async () => {
    try {
      const res = await fetch("http://localhost:5108/api/logs");
      const data = await res.json();
      setLogs(data);
    } catch (err) {
      console.log(err);
    }
  };

  const fetchIncidents = async () => {
    try {
      const res = await fetch("http://localhost:5018/api/incidents");
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
        <h1 style={styles.title}>Incident Intelligence Dashboard</h1>
        <p style={styles.subtitle}>Real-time microservice monitoring system</p>
      </div>

      {/* GRID */}
      <div style={styles.grid}>
        
        {/* INCIDENTS PANEL */}
        <div style={styles.panel}>
          <h2 style={styles.panelTitle}>Active Incidents</h2>

          {incidents.length === 0 ? (
            <p style={styles.empty}>No active incidents</p>
          ) : (
            incidents.map((i, index) => (
              <div key={index} style={styles.incidentCard}>
                <div style={styles.row}>
                  <span style={styles.service}>{i.serviceName}</span>
                  <span style={styles.severity}>{i.severity}</span>
                </div>
                <p style={styles.message}>{i.message}</p>
              </div>
            ))
          )}
        </div>

        {/* LOGS PANEL */}
        <div style={styles.panel}>
          <h2 style={styles.panelTitle}>System Logs</h2>

          {logs.length === 0 ? (
            <p style={styles.empty}>No logs available</p>
          ) : (
            logs.map((l, index) => (
              <div key={index} style={styles.logCard}>
                <span style={styles.logService}>{l.serviceName}</span>
                <span style={styles.logMessage}>{l.message}</span>
              </div>
            ))
          )}
        </div>

      </div>
    </div>
  );
}

/* PROFESSIONAL DARK MINIMAL STYLES */
const styles = {
  container: {
    fontFamily: "system-ui, -apple-system, Segoe UI, Roboto, Arial",
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
    fontSize: "22px",
    fontWeight: "600",
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
    borderRadius: "10px",
    padding: "15px",
    height: "75vh",
    overflowY: "auto"
  },

  panelTitle: {
    fontSize: "15px",
    marginBottom: "10px",
    color: "#f3f4f6",
    borderBottom: "1px solid #1f2937",
    paddingBottom: "8px"
  },

  incidentCard: {
    backgroundColor: "#0b1220",
    border: "1px solid #1f2937",
    padding: "10px",
    borderRadius: "8px",
    marginBottom: "10px"
  },

  row: {
    display: "flex",
    justifyContent: "space-between",
    marginBottom: "5px"
  },

  service: {
    fontWeight: "600",
    fontSize: "13px"
  },

  severity: {
    fontSize: "11px",
    padding: "2px 6px",
    borderRadius: "4px",
    backgroundColor: "#991b1b",
    color: "white"
  },

  message: {
    fontSize: "12px",
    color: "#d1d5db"
  },

  logCard: {
    fontSize: "12px",
    padding: "8px",
    borderBottom: "1px solid #1f2937",
    display: "flex",
    justifyContent: "space-between"
  },

  logService: {
    color: "#93c5fd"
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