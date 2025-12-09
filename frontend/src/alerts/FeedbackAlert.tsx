import { Alert } from "react-bootstrap";

interface FeedbackAlertProps {
  error?: string | null;
  success?: string | null;
  className?: string;
}

export default function FeedbackAlert({
  error,
  success,
  className,
}: FeedbackAlertProps) {
  if (error)
    return (
      <Alert variant="danger" className={className}>
        {error}
      </Alert>
    );
  if (success)
    return (
      <Alert variant="success" className={className}>
        {success}
      </Alert>
    );
  return null;
}
