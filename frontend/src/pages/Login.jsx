import { useState } from "react";
import { useAuth } from "../context/AuthContext";
import { apiRequest } from "../api/api";
import { useNavigate } from "react-router-dom";
import { Link } from "react-router-dom";

export default function Login() {
  const { login } = useAuth();
  const navigate = useNavigate();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  async function handleSubmit(e) {
    e.preventDefault();

    try {
      const data = await apiRequest("/auth/login", "POST", {
        email,
        password
      });

      login(data.token);

      navigate("/");
    } catch (err) {
      alert("Login failed");
    }
  }

  return (
    <div>
      <h2>Login</h2>

      <form onSubmit={handleSubmit}>
        <input
          type="email"
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />

        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />

        <button type="submit">Login</button>

        <p>
            Don't have an account? <Link to="/register">Register</Link>
        </p>
      </form>
    </div>
  );
}