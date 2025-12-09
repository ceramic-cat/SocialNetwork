import { useState } from "react";
import { Form, Button, InputGroup } from "react-bootstrap";
import { useEditProfile } from "../hooks/useEditProfile";
import ProfileAlert from "../alerts/FeedbackAlert";

interface EditProfileFormProps {
  onSuccess?: () => void;
}

export default function EditProfileForm({ onSuccess }: EditProfileFormProps) {
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [editingField, setEditingField] = useState<
    "username" | "email" | "password" | null
  >(null);

  const { loading, error, success, editProfile, resetMessages } =
    useEditProfile(() => {
      setEditingField(null);
      setPassword("");
      if (onSuccess) onSuccess();
    });

  async function handleSubmit(e?: React.FormEvent) {
    if (e) e.preventDefault();
    resetMessages();
    if (editingField === "username" && username)
      await editProfile("username", username);
    if (editingField === "email" && email) await editProfile("email", email);
    if (editingField === "password" && password)
      await editProfile("password", password);
  }

  return (
    <Form>
      <ProfileAlert error={error} success={success} />

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
                resetMessages();
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
                resetMessages();
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
                resetMessages();
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
