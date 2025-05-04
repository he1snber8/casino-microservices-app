import { useEffect, useState } from "react";
import { useWalletHub } from "../hooks/useWalletHub";

export default function WalletPanel({ token }) {
  const [balance, setBalance] = useState(null);
  const [amount, setAmount] = useState("");
  // const [transactionId, setTransactionId] = useState("");
  const [message, setMessage] = useState("");
  const [transactions, setTransactions] = useState([]);

  const API_URL = "http://localhost:5063/wallet";
  const userId = new URLSearchParams(window.location.search).get("userId");

  const walletHub = useWalletHub(token, {
    onTransactionRolledBack: (data) => {
      console.log("Transaction rolled back:", data);

      setTransactions((prev) =>
        prev.map((t) =>
          t.transactionId === data.transactionId
            ? { ...t, rolledBack: true }
            : t
        )
      );
    },
  });

  useEffect(() => {
    const connection = walletHub.current;
    if (!connection) return;

    const handleTxAdded = (tx) => {
      // if (tx.message) {
      //   setMessage(tx.message);
      //   console.error("Transaction error:", tx.message);
      //   return;
      // }

      setTransactions((prev) => {
        const existingIndex = prev.findIndex(
          (t) => t.transactionId === tx.transactionId
        );
        if (existingIndex !== -1) {
          const updated = [...prev];
          updated[existingIndex] = { ...prev[existingIndex], ...tx };
          return updated;
        }
        return [tx, ...prev];
      });

      walletHub.current?.invoke("GetBalance", userId).then((bal) => {
        setBalance(bal);
      });
    };

    connection.on("TransactionAdded", handleTxAdded);

    return () => {
      connection.off("TransactionAdded", handleTxAdded);
    };
  }, [walletHub.current]);

  const handleFillBalance = () => {
    if (!amount) return;
    walletHub.current?.invoke("FillUpBalance", userId, parseFloat(amount));
  };

  const handleGetBalance = () => {
    walletHub.current?.invoke("GetBalance", userId).then((bal) => {
      setBalance(bal);
      setMessage("Balance updated");
    });
  };

  const handleRollback = async (tx) => {
    try {
      await walletHub.current?.invoke(
        "RollbackTransaction",
        userId,
        tx.transactionId
      );
      const newBalance = await walletHub.current?.invoke("GetBalance", userId);

      setBalance(newBalance);
    } catch (error) {
      console.error("Refund failed:", error);
      setMessage("Refund failed");
    }
  };

  return (
    <div className="max-w-xl mx-auto p-8 bg-white rounded-xl shadow-lg font-sans">
      <h1 className="text-3xl font-bold text-gray-900 mb-6">
        Account: {userId}
      </h1>
      <h2 className="text-2xl font-semibold text-gray-800 mb-5">
        💼 My Wallet
      </h2>

      <div className="bg-gray-100 p-4 rounded-lg mb-6 font-medium text-gray-700">
        Current Balance:
        <span
          className={`ml-2 font-semibold ${
            balance == null
              ? "text-gray-500"
              : balance > 0
              ? "text-green-700"
              : "text-red-700"
          }`}
        >
          {balance != null ? `${balance} ₾` : "—"}
        </span>
      </div>

      <div className="flex flex-wrap gap-3 mb-6">
        <button
          className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 transition"
          onClick={handleGetBalance}
        >
          Show Balance
        </button>
        <button
          className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 transition"
          onClick={handleFillBalance}
        >
          Fill Balance
        </button>
      </div>

      <h3 className="text-gray-700 mb-3">Recent Transactions</h3>
      <ul className="list-none max-h-40 overflow-y-auto border border-gray-200 rounded-md px-3 mb-6">
        {transactions.length === 0 ? (
          <li className="py-3 text-gray-400">No transactions yet.</li>
        ) : (
          transactions.map((tx, i) => (
            <li
              key={i}
              className="py-2 border-b flex justify-between items-center border-gray-200 text-gray-800 last:border-b-0"
            >
              💸 <strong>{tx.amount} ₾</strong>
              <small>{tx.transactionId}</small>
              <button
                className={`px-2 py-1 rounded transition ${
                  !tx.rolledBack
                    ? "bg-orange-500 text-white hover:bg-orange-600"
                    : "bg-gray-400 text-white cursor-not-allowed"
                }`}
                onClick={() => !tx.rolledBack && handleRollback(tx)}
                disabled={tx.rolledBack}
              >
                {tx.rolledBack ? "Refunded" : "Refund"}
              </button>
            </li>
          ))
        )}
      </ul>

      <div className="mb-4 space-y-3">
        <input
          type="number"
          placeholder="Amount"
          value={amount}
          onChange={(e) => setAmount(e.target.value)}
          className="w-full px-4 py-2 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-300"
        />
      </div>

      {message && (
        <p className="bg-blue-50 border-l-4 border-blue-500 text-gray-800 rounded px-4 py-3 text-sm">
          {message}
        </p>
      )}
    </div>
  );
}
