import { Alert, Spinner } from "react-bootstrap";
import FollowButton from "../partials/FollowButton";
import useGetUsername from "../hooks/useGetUsername";
import { useGetUserFollowStats } from "../hooks/useGetUserFollowStats";

type OtherUserProfileProps = {
  userId: string;
};

export default function OtherUserProfile({ userId }: OtherUserProfileProps) {
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
          <h1 className="user-header">Welcome to {username}</h1>
          <FollowButton userId={userId} />
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
