import { useLobbyHub } from "../hooks/useLobbyHub";
import { useEffect, useState } from "react";
import WalletPanel from "./Wallet";
import { jwtDecode } from "jwt-decode";
import { useNavigate } from "react-router-dom";

export default function Lobby({ token, onTableId }) {
  const [games, setGames] = useState([]);
  const [tables, setTables] = useState([]);
  const [activeInput, setActiveInput] = useState(null);
  const [inputValue, setInputValue] = useState("");
  const [approvalDropdowns, setApprovalDropdowns] = useState({});

  const decoded = jwtDecode(token);
  const userId = decoded?.nameid || "Anonymous";
  const navigate = useNavigate();

  const hubRef = useLobbyHub(token, {
    GameTableCreated: async (table) => {
      if (hubRef.current) {
        try {
          setTables((prev) => {
            const tableExists = prev.some((t) => t.id === table.id);
            if (!tableExists) {
              return [...prev, table]; // Only append new table if not already present
            }
            return prev; // Otherwise, return previous state unchanged
          });
        } catch (err) {
          console.error("Failed to fetch tables:", err);
        }
      }
    },

    PlayerJoined: (tableId, player) =>
      console.log(`${player} joined ${tableId}`),

    AvailableGames: (games) => setGames(games),

    AvailableTables: (tables) => setTables(tables || []),

    JoinApproved: (table) => {
      console.log("Join approved for table:", table);
      // setTables((prev) => {
      //   const updatedTables = prev.map((t) =>
      //     t.id === table.id ? { ...t, players: table.players } : t
      //   );
      //   // // If the table is not found in the current state, add it as a new one
      //   // if (!updatedTables.find((t) => t.id === table.id)) {
      //   //   return [...updatedTables, table];
      //   // }
      //   return updatedTables;
      // });
    },

    GameCancelled: (tableId) => console.log(`Game ${tableId} cancelled`),
  });

  useEffect(() => {
    const fetchInitialData = async () => {
      const connection = hubRef.current;
      if (!connection) {
        console.warn("Hub connection not ready yet");
        return;
      }

      try {
        console.log("Fetching available tables...");
        const games = await connection.invoke("GetAvailableGames");

        if (!games || !Array.isArray(games)) {
          console.warn("GetAvailableGames returned no games:", games);
          return;
        }

        const parsedGames = games.map((game) => ({
          ...game,
          Rules:
            game.Rules?.map((rule) => ({
              ...rule,
              ValueList:
                typeof rule.valueType === "string" &&
                rule.valueType.includes(";")
                  ? rule.valueType.split(";").map((s) => s.trim())
                  : [rule.valueType],
            })) || [],
        }));
        console.log("Available games:", parsedGames);
        setGames(parsedGames);
      } catch (err) {
        console.error("Initial data fetch failed:", err);
      }
    };

    fetchInitialData();
  }, [token]);

  return (
    <div className="p-8 space-y-8">
      <WalletPanel token={token} />
      <div>
        <h2 className="text-3xl font-bold mb-4">🎮 Game Lobby</h2>
        <h3 className="text-xl font-semibold mb-2">Available Games:</h3>
        <div className="flex flex-col gap-4">
          {games.map((game) => (
            <div
              key={game.gameName}
              className="border-4 border-gray-300 rounded-lg p-6 bg-white shadow-md"
            >
              <h1 className="w-full mb-4 p-4 rounded-md bg-gray-100 cursor-pointer text-lg font-semibold">
                {game.gameName}
              </h1>

              {game.rules && game.rules.length > 0 && (
                <ul className="ml-4 bg-re space-y-4">
                  {game.rules.map((rule, index) => (
                    <li key={index} className="flex flex-col gap-2">
                      <strong className="text-gray-700">
                        {rule.ruleName}:
                      </strong>
                      {Array.isArray(rule.valueType)
                        ? rule.valueType.map((value, idx) => (
                            <div key={idx} className="w-full">
                              {activeInput === `${rule.ruleName}-${idx}` ? (
                                <div className="flex gap-2">
                                  <input
                                    type="number"
                                    placeholder={`Enter stake between ${value}`}
                                    value={inputValue}
                                    onChange={(e) =>
                                      setInputValue(e.target.value)
                                    }
                                    className="p-2 rounded border border-gray-300 w-full"
                                  />
                                  <button
                                    onClick={async () => {
                                      if (!hubRef.current) return;
                                      const tableRequest = {
                                        gameName: game.gameName,
                                        createdBy: userId,
                                        entryFee: parseFloat(inputValue),
                                        rules: {
                                          [rule.ruleName]: inputValue,
                                        },
                                      };
                                      try {
                                        await hubRef.current.invoke(
                                          "CreateGameTable",
                                          tableRequest
                                        );
                                      } catch (err) {
                                        console.error(
                                          "Failed to create game table:",
                                          err
                                        );
                                      }
                                    }}
                                    className="px-4 py-2 bg-blue-500 text-white font-semibold rounded"
                                  >
                                    Enter
                                  </button>
                                </div>
                              ) : (
                                <span
                                  onClick={() =>
                                    setActiveInput(`${rule.ruleName}-${idx}`)
                                  }
                                  className="inline-block w-full p-2 rounded bg-gray-200 cursor-pointer font-semibold"
                                >
                                  {value}
                                </span>
                              )}
                            </div>
                          ))
                        : rule.valueType}
                    </li>
                  ))}
                </ul>
              )}
            </div>
          ))}
        </div>
      </div>

      <div>
        <h3 className="text-xl font-semibold mt-8 mb-4">🪑 Live Tables:</h3>
        <ul className="space-y-4">
          {tables.map((table) => (
            <li
              key={table.id}
              className="flex justify-between items-center p-6 border border-gray-300 rounded-lg bg-white shadow"
            >
              <div>
                <div className="text-lg font-bold text-gray-800">
                  {table.gameName}
                </div>
                <div className="text-sm text-gray-600">
                  Created by{" "}
                  <span className="font-medium">{table.createdBy}</span>
                </div>
                <div className="text-sm text-gray-600">
                  Entry fee:{" "}
                  <span className="text-green-600 font-medium">
                    ₾{table.entryFee}
                  </span>{" "}
                  | Players: {table.players.length}/2
                </div>
              </div>

              <div className="flex flex-col items-end gap-2">
                {userId === table.createdBy && table.players.length < 2 ? (
                  <div className="relative inline-block text-left">
                    <button
                      onClick={async () => {
                        // if (!hubRef.current) return;
                        console.log("Fetching pending approvals...");
                        const approvals = await hubRef.current.invoke(
                          "GetPendingApprovals",
                          table.id,
                          userId
                        );
                        console.log("Pending approvals:", approvals);
                        setApprovalDropdowns((prev) => ({
                          ...prev,
                          [table.id]: {
                            open: !prev[table.id]?.open,
                            approvals,
                          },
                        }));
                      }}
                      className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
                    >
                      See Pending Approvals
                    </button>

                    {approvalDropdowns[table.id]?.open && (
                      <div className="absolute z-10 mt-2 w-64 bg-white border rounded shadow-md">
                        {approvalDropdowns[table.id].approvals.length > 0 ? (
                          approvalDropdowns[table.id].approvals.map(
                            (pendingUserId) => (
                              <div
                                key={pendingUserId}
                                className="flex items-center justify-between px-4 py-2 hover:bg-gray-100"
                              >
                                <span className="text-gray-700">
                                  {pendingUserId}
                                </span>
                                <button
                                  onClick={async () => {
                                    console.log(
                                      "Invoking ApproveJoin with tableId:",
                                      table.id,
                                      "and player:",
                                      pendingUserId
                                    );
                                    await hubRef.current.invoke(
                                      "ApproveJoin",
                                      table.id,
                                      pendingUserId
                                    );
                                    setApprovalDropdowns((prev) => ({
                                      ...prev,
                                      [table.id]: {
                                        ...prev[table.id],
                                        approvals: prev[
                                          table.id
                                        ].approvals.filter(
                                          (u) => u !== pendingUserId
                                        ),
                                      },
                                    }));
                                  }}
                                  className="ml-2 px-2 py-1 text-sm bg-green-500 text-white rounded hover:bg-green-600"
                                >
                                  Approve
                                </button>
                              </div>
                            )
                          )
                        ) : (
                          <div className="px-4 py-2 text-gray-500">
                            No pending approvals
                          </div>
                        )}
                      </div>
                    )}
                  </div>
                ) : table.players.length >= 2 ? (
                  <div className="flex items-center gap-8">
                    <button
                      onClick={() => {
                        onTableId(table.id);
                        navigate("/games/coinflip");
                      }}
                      className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-md font-medium"
                    >
                      Return to game
                    </button>
                    <span className="text-red-500">Game room is filled</span>
                  </div>
                ) : table.players.includes(userId) ? (
                  <span className="text-green-500">You are already in</span>
                ) : table.players.length === 0 ? (
                  <span className="text-gray-500">Waiting for players...</span>
                ) : table.players.length === 1 ? (
                  <span className="text-gray-500">
                    Waiting for another player...
                  </span>
                ) : null}
              </div>
              {!table.players.includes(userId) &&
                table.players.length < 2 &&
                userId !== table.createdBy && (
                  <button
                    onClick={async () => {
                      try {
                        console.log(
                          "Invoking JoinTable with tableId:",
                          table.id,
                          "and userId:",
                          userId
                        );
                        await hubRef.current.invoke(
                          "JoinTable",
                          table.id,
                          userId
                        );
                        window.alert(
                          `✅ You successfully sent join request: ${table.gameName}`
                        );
                      } catch (error) {
                        console.error("Failed to join table:", error);
                        window.alert(
                          "❌ Failed to join the table. Please try again."
                        );
                      }
                    }}
                    className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-md font-medium"
                  >
                    Join
                  </button>
                )}
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
}
