import React, { useState } from "react";
import { useWalletHub } from "../hooks/useWalletHub";

export default function WalletNotifications(token) {
  const [logs, setLogs] = useState([]);

  useWalletHub(token, {
    onBalanceCredited: (data) =>
      setLogs((prev) => [
        `Credited: +${data.Amount} | New: ${data.NewBalance}`,
        ...prev,
      ]),
    onBalanceDebited: (data) =>
      setLogs((prev) => [
        `Debited: -${data.Amount} | New: ${data.NewBalance}`,
        ...prev,
      ]),
    onTransactionRolledBack: (data) =>
      setLogs((prev) => [`Rollback TX: ${data.TransactionId}`, ...prev]),
  });

  return (
    <div style={{ padding: "1rem" }}>
      <h3>Wallet Notifications</h3>
      <ul>
        {logs.map((log, i) => (
          <li key={i}>{log}</li>
        ))}
      </ul>
    </div>
  );
}
