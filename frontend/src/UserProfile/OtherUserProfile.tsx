import { Alert, Spinner, Button } from "react-bootstrap";
import FollowButton from "../partials/FollowButton";
import useGetUsername from "../hooks/useGetUsername";
import { useGetUserFollowStats } from "../hooks/useGetUserFollowStats";

type OtherUserProfileProps = {
  userId: string;
  onSendMessage?: (receiverId: string) => void;
};

export default function OtherUserProfile({
  userId,
  onSendMessage,
}: OtherUserProfileProps) {
  const { error, isLoading, username } = useGetUsername(userId);
  const {
    stats,
    loading: statsLoading,
    error: statsError,
  } = useGetUserFollowStats(userId);

  return (
    <div>
      {isLoading ? (
        <Spinner />
      ) : error ? (
        <Alert>{error}</Alert>
      ) : (
        <div className="user-container">
          <div className="d-flex align-items-center justify-content-center gap-2 mb-3">
            <h1 className="user-header mb-0">Welcome to {username}</h1>
            {onSendMessage && (
              <Button
                variant="secondary"
                onClick={() => onSendMessage(userId)}
                className="p-2"
                title="Send message"
                style={{ minWidth: "auto" }}
              >
                <i className="bi bi-chat-left-text" />
              </Button>
            )}
          </div>
          <div className="d-flex justify-content-center mb-3">
            <FollowButton userId={userId} />
          </div>
          {statsLoading ? (
            <Spinner />
          ) : statsError ? (
            <Alert>{statsError}</Alert>
          ) : (
            <div className="user-stats-box">
              <div className="user-stats-item">
                <span className="user-stats">Followers {stats.followers}</span>
              </div>
              <div className="user-stats-item">
                <span className="user-stats">Following {stats.following}</span>
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
