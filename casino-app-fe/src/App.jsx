import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
} from "react-router-dom";
import { useState } from "react";
import Login from "./components/Login";
import Lobby from "./components/Lobby";
import CoinFlip from "./components/CoinFlip";

function App() {
  const [token, setToken] = useState(null);
  const [tableId, setTableId] = useState(null);

  console;

  return (
    <Router>
      <Routes>
        <Route path="/" element={<Login onToken={setToken} />} />
        <Route
          path="/lobby"
          element={
            token ? (
              <Lobby token={token} onTableId={setTableId} />
            ) : (
              <Navigate to="/" />
            )
          }
          // children={token ? <CoinFlip token={token} /> : <Navigate to="/" />}
        />
        <Route
          path="/games/coinflip"
          element={
            token ? (
              <CoinFlip token={token} tableId={tableId} />
            ) : (
              <Navigate to="/" />
            )
          }
        />
      </Routes>
    </Router>
  );
}

export default App;
