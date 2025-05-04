import { useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";

export function useWalletHub(token, handlers) {
  const hubRef = useRef(null);

  useEffect(() => {
    const connect = async () => {
      const connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5063/wallethub", {
          accessTokenFactory: () => token,
        })
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();

      connection.on("BalanceAltered", (amount) => {
        console.log("Balance filled:", amount);
        handlers.onBalanceAltered?.(amount);
      });

      connection.on("TransactionRolledBack", (data) => {
        handlers.onTransactionRolledBack?.(data);
      });

      connection.onclose((err) => {
        console.error("SignalR connection closed:", err);
      });

      try {
        await connection.start();
        console.log("WalletHub Connected ✅ ");
        hubRef.current = connection;
      } catch (err) {
        console.error("❌ Connection failed:", err);
      }
    };

    connect();

    return () => {
      if (hubRef.current) {
        hubRef.current.stop();
      }
    };
  }, [token]);

  return hubRef;
}
