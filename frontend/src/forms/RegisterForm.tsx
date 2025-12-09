import { useState } from "react";
import { Button, Form, Alert } from "react-bootstrap";
import { useAuth } from "../hooks/useAuth";
import FeedbackAlert from "../alerts/FeedbackAlert";

interface RegisterFormProps {
  onSuccess: () => void;
  onSwitchToLogin?: () => void;
}

export default function RegisterForm({
  onSuccess,
  onSwitchToLogin,
}: RegisterFormProps) {
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [validationError, setValidationError] = useState("");
  const { register, loading, error, resetError } = useAuth();

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    resetError();
    setValidationError("");

    if (!username || !email || !password || !confirmPassword) {
      setValidationError("All fields are required.");
      return;
    }
    if (!/\S+@\S+\.\S+/.test(email)) {
      setValidationError("Please enter a valid email address.");
      return;
    }
    if (password !== confirmPassword) {
      setValidationError("Passwords do not match");
      return;
    }

    const success = await register({ username, email, password });
    if (success) {
      if (onSwitchToLogin) {
        onSwitchToLogin();
      }
    }
  }

  return (
    <Form onSubmit={handleSubmit} noValidate>
      <FeedbackAlert
        error={validationError || error}
        className="alert-form-danger"
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
        <Form.Label>Email</Form.Label>
        <Form.Control
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
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

      <Form.Group className="mb-3">
        <Form.Label>Confirm Password</Form.Label>
        <Form.Control
          type="password"
          value={confirmPassword}
          onChange={(e) => setConfirmPassword(e.target.value)}
          required
        />
      </Form.Group>

      <Button type="submit" variant="primary" disabled={loading}>
        {loading ? "Registering..." : "Register"}
      </Button>
    </Form>
  );
}
