import { Alert } from "react-bootstrap";

interface ProfileAlertProps {
  error?: string | null;
  success?: string | null;
}

export default function ProfileAlert({ error, success }: ProfileAlertProps) {
  if (error) return <Alert variant="danger">{error}</Alert>;
  if (success) return <Alert variant="success">{success}</Alert>;
  return null;
}
