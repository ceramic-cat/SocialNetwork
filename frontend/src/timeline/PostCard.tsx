import { Button, Card } from "react-bootstrap";
import "../../sass/_postCard.scss";

type PostCardProps = {
  sender: string;
  content: string;
  timestamp: string;
  canDelete?: boolean;
  onDelete?: () => void;
  deleting?: boolean;
};

export default function PostCard({
  sender,
  content,
  timestamp,
  canDelete = false,
  onDelete,
  deleting = false,
}: PostCardProps) {
  return (
    <Card className="post-card">
      <Card.Header>{sender}</Card.Header>

      <Card.Body>
        <div className="post-content">{content}</div>

        <div className="timestamp">{timestamp}</div>
      </Card.Body>
      <Card.Footer>
        {canDelete && onDelete && (
          <Button
            variant="outline-danger"
            size="sm"
            onClick={onDelete}
            disabled={deleting}
          >
            {deleting ? "Deleting..." : "Delete"}
          </Button>
        )}
      </Card.Footer>
    </Card>
  );
}
