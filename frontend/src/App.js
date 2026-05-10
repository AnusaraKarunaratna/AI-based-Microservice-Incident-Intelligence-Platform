import { useEffect, useState} from "react";

function App() {

  const [incidents, setIncidents] = useState([]);
  const [logs, setLogs] = useState([]);

  useEffect(() => {
      const interval = setInterval(() =>{
        fetchLogs();
        fetchIncidents();
      },3000);
      return () => clearInterval(interval);
  },[]);

  const fetchLogs = async () => {
    try{
      const res = await fetch("http://localhost:5108/api/logs");
      const data = await res.json();
      setLogs(data);
    }catch(err){
      console.log(err);
    }
  }

  const fetchIncidents = async () => {
    try{
      const res = await fetch("http://localhost:5108/api/incidents");
      const data = await res.json();
      setIncidents(data);
    }catch(err){
      console.log(err);
    }
  }
  return (
    <div style={{ padding: "20px", fontFamily: "Arial" }}>
      <h1>Incident Intelligence Dashboard</h1>

      <h2>Active Incidents</h2>
      {incidents.map((i, index) => (
        <div key={index} style={{ border: "1px solid red", margin: 10, padding: 10 }}>
          <h3>{i.serviceName}</h3>
          <p>{i.message}</p>
          <b>Severity: {i.severity}</b>
        </div>
      ))}

      <h2>Logs</h2>
      {logs.map((l, index) => (
        <div key={index} style={{ border: "1px solid gray", margin: 10, padding: 10 }}>
          <p>{l.serviceName} - {l.message}</p>
        </div>
      ))}
    </div>
  );
}

export default App;
