import { useEffect, useState } from "react";
import { useLobbyHub } from "../hooks/useLobbyHub";
import { jwtDecode } from "jwt-decode";
import { useNavigate } from "react-router-dom";

export default function CoinFlip({ token, tableId }) {
  const [tableInfo, setTableInfo] = useState(null);
  const [winner, setWinner] = useState(null);
  const [flipping, setFlipping] = useState(false);

  // console.log("Fetching table info for table ID:", tableId);
  // // console.log("Hub reference:", hubRef.current);
  // console.log("Token:", token);
  const decoded = jwtDecode(token);
  const userId = decoded?.nameid || "Anonymous";

  const hubRef = useLobbyHub(token);
  const navigate = useNavigate();

  useEffect(() => {
    const interval = setInterval(() => {
      if (hubRef.current) {
        hubRef.current
          .invoke("GetTableInformation", tableId)
          .then((table) => {
            if (!table || table.gameName === "NOT FOUND") {
              console.warn("Table not found");
              return;
            }
            setTableInfo(table);
          })
          .catch((err) => {
            console.error("Error getting table info:", err);
          })
          .finally(() => clearInterval(interval)); // stop polling once done
      }
    }, 500);

    return () => clearInterval(interval);
  }, [hubRef, tableId]);

  const handleFlip = () => {
    setFlipping(true);
    setWinner(null);

    setTimeout(() => {
      const result = Math.random() < 0.5 ? "Player 1" : "Player 2";
      setWinner(result);
      setFlipping(false);
    }, 1000); // 1 second flip delay
  };

  return (
    <div className="p-6 flex justify-center items-center min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100">
      {tableInfo ? (
        <div className="bg-white rounded-2xl shadow-2xl p-8 w-full max-w-md border border-indigo-200">
          <h1 className="text-3xl font-extrabold text-indigo-600 text-center mb-4">
            🎯 CoinFlip Game
          </h1>

          <div className="flex items-center justify-center mb-6">
            <div className="flex flex-col items-center mx-4">
              <div className="w-12 h-12 bg-indigo-200 rounded-full flex items-center justify-center text-white font-bold">
                P1
              </div>
              <span className="mt-2 text-sm text-gray-700">Player 1</span>
            </div>
            <div className="text-4xl animate-bounce">🪙</div>
            <div className="flex flex-col items-center mx-4">
              <div className="w-12 h-12 bg-pink-300 rounded-full flex items-center justify-center text-white font-bold">
                P2
              </div>
              <span className="mt-2 text-sm text-gray-700">Player 2</span>
            </div>
          </div>

          <div className="space-y-3 text-sm text-gray-800 mb-4">
            <p>
              <span className="font-semibold text-indigo-500">Entry Fee:</span>{" "}
              <span className="text-green-600 font-medium">
                {tableInfo.entryFee} ₾
              </span>
            </p>
          </div>

          <button
            onClick={handleFlip}
            disabled={flipping}
            className="w-full bg-indigo-600 text-white py-2 px-4 rounded hover:bg-indigo-700 transition disabled:opacity-50"
          >
            {flipping ? "Flipping..." : "Flip Coin 🪙"}
          </button>
          <button
            onClick={() => {
              navigate(`/lobby?userId=${userId}`);
              hubRef.current.invoke("QuitGame", tableId, userId);
            }}
            disabled={flipping}
            className="w-full mt-4 bg-red-600 text-white py-2 px-4 rounded hover:bg-red-900 transition disabled:opacity-50"
          >
            Cancel
          </button>

          {winner && (
            <div className="mt-6 text-center">
              <p className="text-xl font-bold text-green-600 animate-pulse">
                🎉 {winner} wins the coin flip!
              </p>
            </div>
          )}
        </div>
      ) : (
        <div className="text-lg text-gray-500 animate-pulse">
          Loading table information...
        </div>
      )}
    </div>
  );
}
