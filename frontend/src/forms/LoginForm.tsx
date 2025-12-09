import { useState } from "react";
import { Button, Form, Alert } from "react-bootstrap";
import { useAuth } from "../hooks/useAuth";
import FeedbackAlert from "../alerts/FeedbackAlert";

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
  const [validationError, setValidationError] = useState("");
  const { login, loading, error, resetError } = useAuth();

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    resetError();
    setValidationError("");

    if (!username || !password) {
      setValidationError("Both fields are required.");
      return;
    }

    const success = await login({ username, password });
    if (success) {
      onSuccess();
    }
  }

  return (
    <Form onSubmit={handleSubmit} noValidate>
      <FeedbackAlert
        error={validationError || error}
        className="alert-form-danger"
      />
      <FeedbackAlert
        success={
          showRegistrationSuccess ? "Registration successful!" : undefined
        }
      />

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
