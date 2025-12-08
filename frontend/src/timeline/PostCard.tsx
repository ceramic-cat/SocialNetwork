import { Button, Card } from "react-bootstrap";
import "../../sass/_postCard.scss";
import type { ReactNode } from "react";

type PostCardProps = {
  sender: ReactNode;
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

        <div className="post-footer">
          <span className="timestamp">{timestamp}</span>

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
        </div>
      </Card.Body>
    </Card>
  );
}
