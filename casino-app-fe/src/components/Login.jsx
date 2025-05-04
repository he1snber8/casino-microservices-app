import { useState } from "react";
import { useNavigate } from "react-router-dom";

export default function Login({ onToken }) {
  const [username, setUsername] = useState("player1");
  const [password, setPassword] = useState("password1");
  const navigate = useNavigate();

  const handleLogin = async () => {
    const res = await fetch("http://localhost:5062/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ username, password }),
    });

    if (res.ok) {
      const { token } = await res.json();
      onToken(token);
      navigate(`/lobby?userId=${username}`); // <--- redirect
    } else {
      alert("Login failed");
    }
  };

  return (
    <div
      style={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        height: "100vh",
        fontFamily: "Arial, sans-serif",
        backgroundColor: "#f4f4f4",
      }}
    >
      <h2 style={{ marginBottom: "20px", color: "#333" }}>Login</h2>
      <input
        value={username}
        onChange={(e) => setUsername(e.target.value)}
        placeholder="Username"
        style={{
          padding: "10px",
          marginBottom: "10px",
          width: "250px",
          borderRadius: "4px",
          border: "1px solid #ccc",
          fontSize: "16px",
        }}
      />
      <input
        type="password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        placeholder="Password"
        style={{
          padding: "10px",
          marginBottom: "20px",
          width: "250px",
          borderRadius: "4px",
          border: "1px solid #ccc",
          fontSize: "16px",
        }}
      />
      <button
        onClick={handleLogin}
        style={{
          padding: "10px 20px",
          backgroundColor: "#007BFF",
          color: "#fff",
          border: "none",
          borderRadius: "4px",
          fontSize: "16px",
          cursor: "pointer",
        }}
      >
        Login
      </button>
    </div>
  );
}
