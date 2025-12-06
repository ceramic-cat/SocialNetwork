import { Card } from "react-bootstrap";
import "../../sass/_postCard.scss";

type PostCardProps = {
  sender: string;
  content: string;
  timestamp: string;
};

export default function PostCard({
  sender,
  content,
  timestamp,
}: PostCardProps) {
  return (
    <Card className="post-card">
      <Card.Header>{sender}</Card.Header>

      <Card.Body>
        <div className="post-content">{content}</div>

        <div className="timestamp">{timestamp}</div>
      </Card.Body>
    </Card>
  );
}
