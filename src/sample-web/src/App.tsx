import axios from "axios";
import { useEffect, useState } from "react";
import "./App.css";
import viteLogo from "/vite.svg";

interface ExampleResponse {
  appId: number;
  records: ExampleRecord[];
}
interface ExampleRecord {
  id: number;
  description: string;
}

function App() {
  const [appId, setAppId] = useState(0);
  const [data, setData] = useState([] as ExampleRecord[]);
  const [newRecord, setNewRecord] = useState("");

  const fetchData = async () => {
    // get url from env variable
    const url = import.meta.env.VITE_API_URL;
    const response = await axios.get(`${url}/sample`);
    console.log(response.data.records);
    if (response.status === 200) {
      setData(response.data.records);
      setAppId(response.data.appId);
    }
  };

  const handleSubmit = async () => {
    const url = import.meta.env.VITE_API_URL;
    if (!newRecord) return;
    const response = await axios.post(`${url}/sample`, {
      description: newRecord,
    });
    console.log(response.data);
    if (response.status === 201) {
      fetchData();
      setNewRecord("");
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  return (
    <>
      <div>
        <a
          href="https://github.com/gusflopes/Gusflopes.YarpDevInfra"
          target="_blank"
        >
          <img src={viteLogo} className="logo" alt="Vite logo" />
        </a>
      </div>
      <h1>Sample App - Id: {appId ?? appId}</h1>
      <div className="card">
        <h2>Dados</h2>
        <input
          value={newRecord}
          onChange={(e) => setNewRecord(e.currentTarget.value)}
        />
        <button onClick={() => handleSubmit()}>Enviar</button>
        <ul>
          {data &&
            data.map((item) => (
              <li key={item.id}>
                {item.id} - {item.description}
              </li>
            ))}
        </ul>
      </div>
      <p className="read-the-docs">
        Aplicativo de demonstração para validar a infraestrutura de
        desenvolvimento do YARP.
      </p>
    </>
  );
}

export default App;
