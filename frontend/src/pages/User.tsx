import Timeline from "../timeline/Timeline";
import { useParams } from "react-router-dom";
import { Container } from "react-bootstrap";
import useCurrentUser from "../hooks/useCurrentUser";
import OwnUserProfile from "../UserProfile/OwnUserProfile";
import OtherUserProfile from "../UserProfile/OtherUserProfile";
import "../../sass/timeline.scss";

export default function User() {
  const { id: userId } = useParams<{ id: string }>();
  const { userId: currentUserId, username: currentUsername } = useCurrentUser();

  const isOwnPage = userId === currentUserId;

  if (!userId) return <div>User not found</div>;

  return (
    <Container fluid className="px-0 user-page">
      {isOwnPage ? (
        <OwnUserProfile username={currentUsername} />
      ) : (
        <OtherUserProfile userId={userId} />
      )}
      <Timeline />
    </Container>
  );
}
