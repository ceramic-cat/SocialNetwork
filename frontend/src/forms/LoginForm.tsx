import { useState } from "react";
import { Button, Form, Alert } from "react-bootstrap";
import { useAuth } from "../hooks/useAuth";
import { useNavigate } from "react-router-dom";

interface LoginFormProps {
  onSuccess: () => void;
  showRegistrationSuccess?: boolean;
}

export default function LoginForm({
  onSuccess,
  showRegistrationSuccess,
}: LoginFormProps) {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const { login, loading, error, resetError } = useAuth();
  const navigate = useNavigate();

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    resetError();

    const success = await login({ username, password });
    if (success) {
      onSuccess();
      navigate("/");
    }
  }

  return (
    <Form onSubmit={handleSubmit}>
      {showRegistrationSuccess && (
        <div className="text-success mb-3">Registration successful!</div>
      )}
      {error && <Alert variant="danger">{error}</Alert>}

      <Form.Group className="mb-3">
        <Form.Label>Username</Form.Label>
        <Form.Control
          type="text"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          required
        />
      </Form.Group>

      <Form.Group className="mb-3">
        <Form.Label>Password</Form.Label>
        <Form.Control
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
      </Form.Group>

      <Button type="submit" variant="primary" disabled={loading}>
        {loading ? "Logging in..." : "Login"}
      </Button>
    </Form>
  );
}
