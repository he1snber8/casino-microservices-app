import { useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";

export const useLobbyHub = (token, onEvents = {}) => {
  const connectionRef = useRef(null);

  useEffect(() => {
    const connect = async () => {
      try {
        const connection = new signalR.HubConnectionBuilder()
          .withUrl("http://localhost:5064/lobbyhub", {
            accessTokenFactory: () => token,
            withCredentials: false,
          })
          .withAutomaticReconnect()
          .configureLogging(signalR.LogLevel.Information)
          .build();

        // Register all event handlers passed into the hook
        for (const [eventName, handler] of Object.entries(onEvents)) {
          connection.on(eventName, handler);
        }

        // Optionally add a default one here if not passed in onEvents
        if (!onEvents.PlayerJoined) {
          connection.on("PlayerJoined", (tableId, player) => {
            console.log("Player joined:", player, "on table", tableId);
          });
        }

        connection.onclose((err) => {
          if (err) {
            console.error("SignalR connection closed with error:", err.message);
          } else {
            console.log("SignalR connection closed gracefully.");
          }
        });

        await connection.start();
        console.log("✅ SignalR connected");

        // Call initial fetches — handle errors explicitly
        try {
          await connection.invoke("GetAvailableGames");
          await connection.invoke("GetAvailableTables");
        } catch (invokeErr) {
          console.error("Failed to invoke methods after connect:", invokeErr);
        }

        connectionRef.current = connection;
      } catch (err) {
        console.error("❌ SignalR connection error:", err);
      }
    };

    connect();

    return () => {
      if (connectionRef.current) {
        connectionRef.current.stop();
      }
    };
  }, [token]);

  return connectionRef;
};
