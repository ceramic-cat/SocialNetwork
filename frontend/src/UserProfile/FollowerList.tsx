import { useGetFollowersInfo } from "../hooks/useGetFollowersInfo";
import { Alert, ListGroup, Spinner } from "react-bootstrap";
import { useNavigate } from "react-router-dom";

export default function FollowersList() {
  const { isLoading, error, followers } = useGetFollowersInfo();
  const navigate = useNavigate();
  const followersEmpty = followers.length === 0;

  return (
    <div className="container py-4 text-light">
      <h4 className="text-white pt-4">All your followers</h4>
      {isLoading ? (
        <Spinner className="align-center" />
      ) : error ? (
        <Alert>{error}</Alert>
      ) : followersEmpty ? (
        <p>You have no followers.</p>
      ) : (
        <ListGroup>
          {followers.map((follower) => (
            <ListGroup.Item
              key={follower.id}
              action
              onClick={() => navigate(`/users/${follower.id}`)}
              className="bg-transparent text-light border-secondary"
            >
              {follower.username}
            </ListGroup.Item>
          ))}
        </ListGroup>
      )}
    </div>
  );
}
