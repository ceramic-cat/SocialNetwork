import { useState } from "react";
import { Form, Button, Alert, InputGroup } from "react-bootstrap";

const API_URL = "http://localhost:5148/api/auth/edit-profile";

interface EditProfileFormProps {
  onSuccess?: () => void;
}

export default function EditProfileForm({ onSuccess }: EditProfileFormProps) {
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [editingField, setEditingField] = useState<
    "username" | "email" | "password" | null
  >(null);

  async function handleSubmit(e?: React.FormEvent) {
    if (e) e.preventDefault();
    setError(null);
    setSuccess(null);
    setLoading(true);

    let body: any = {};
    if (editingField === "username" && username) body.username = username;
    if (editingField === "email" && email) body.email = email;
    if (editingField === "password" && password) body.password = password;

    try {
      const token = localStorage.getItem("token");
      const response = await fetch(API_URL, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(body),
      });

      if (!response.ok) {
        const data = await response.text();
        setError(data || "Could not update profile.");
      } else {
        setSuccess("Profile updated successfully!");
        setEditingField(null);
        setPassword("");
        if (onSuccess) onSuccess();
      }
    } catch (err) {
      setError("An error occurred. Please try again.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <Form>
      {error && <Alert variant="danger">{error}</Alert>}
      {success && <Alert variant="success">{success}</Alert>}

      <Form.Group className="mb-3">
        <Form.Label>Username</Form.Label>
        {editingField === "username" ? (
          <InputGroup>
            <Form.Control
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
            />
            <Button
              variant="primary"
              disabled={loading}
              onClick={() => handleSubmit()}
            >
              Save
            </Button>
            <Button
              variant="secondary"
              onClick={() => {
                setUsername("");
                setEditingField(null);
              }}
            >
              Cancel
            </Button>
          </InputGroup>
        ) : (
          <InputGroup>
            <Form.Control type="text" value={username} readOnly />
            <Button
              variant="outline-primary"
              onClick={() => setEditingField("username")}
            >
              Edit
            </Button>
          </InputGroup>
        )}
      </Form.Group>

      <Form.Group className="mb-3">
        <Form.Label>Email</Form.Label>
        {editingField === "email" ? (
          <InputGroup>
            <Form.Control
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
            <Button
              variant="primary"
              disabled={loading}
              onClick={() => handleSubmit()}
            >
              Save
            </Button>
            <Button
              variant="secondary"
              onClick={() => {
                setEmail("");
                setEditingField(null);
              }}
            >
              Cancel
            </Button>
          </InputGroup>
        ) : (
          <InputGroup>
            <Form.Control type="email" value={email} readOnly />
            <Button
              variant="outline-primary"
              onClick={() => setEditingField("email")}
            >
              Edit
            </Button>
          </InputGroup>
        )}
      </Form.Group>

      <Form.Group className="mb-3">
        <Form.Label>New Password</Form.Label>
        {editingField === "password" ? (
          <InputGroup>
            <Form.Control
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Enter new password"
            />
            <Button
              variant="primary"
              disabled={loading}
              onClick={() => handleSubmit()}
            >
              Save
            </Button>
            <Button
              variant="secondary"
              onClick={() => {
                setPassword("");
                setEditingField(null);
              }}
            >
              Cancel
            </Button>
          </InputGroup>
        ) : (
          <InputGroup>
            <Form.Control
              type="password"
              value=""
              placeholder="********"
              readOnly
            />
            <Button
              variant="outline-primary"
              onClick={() => setEditingField("password")}
            >
              Edit
            </Button>
          </InputGroup>
        )}
      </Form.Group>
    </Form>
  );
}
