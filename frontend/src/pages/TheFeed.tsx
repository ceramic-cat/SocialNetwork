import { Row, Col, Button } from "react-bootstrap";
import { Link } from "react-router-dom";
import { useNavigate } from "react-router-dom";
import { useFeedPosts } from "../hooks/useFeedPosts";
import { useFollowing } from "../hooks/useFollowing";
import useCurrentUser from "../hooks/useCurrentUser";

export default function TheFeed() {
  const { userId, loading: userLoading } = useCurrentUser();

  const {
    posts,
    loading,
    error: feedError,
  } = useFeedPosts(userId, userLoading);
  const { following, error: followingError } = useFollowing(
    userId,
    userLoading
  );
  const navigate = useNavigate();

  return (
    <Row className="justify-content-center text-center">
      <div className="feed-space-animation"></div>

      <Col xs={12}>
        <h2 className="feed-title">Your Feed</h2>
      </Col>
      <Col xs={12} md={8} lg={6} className="feed-list-container custom-scroll">
        {loading ? (
          <Col className="alert-info">Loading...</Col>
        ) : feedError || followingError ? (
          <Col className="alert-danger">{feedError || followingError}</Col>
        ) : following.length === 0 ? (
          <Col className="alert-following-info">
            <div>You are not following anyone yet.</div>
            <Button
              variant="secondary"
              className="mt-3"
              onClick={() => navigate("/search")}
            >
              Find users to follow
            </Button>
          </Col>
        ) : posts.length === 0 ? (
          <Col className="alert-info">No posts to show.</Col>
        ) : (
          <ul className="feed-list">
            {posts.map((post) => {
              const isNew =
                Date.now() - new Date(post.createdAt).getTime() <
                24 * 60 * 60 * 1000;
              return (
                <li
                  key={post.id}
                  className={`feed-post${isNew ? " new-post" : ""}`}
                >
                  {isNew && <span className="new-badge">New</span>}
                  <Col className="post-date">
                    <i className="bi bi-clock-history" />
                    {new Date(post.createdAt).toLocaleString()}
                  </Col>
                  <Row>
                    <Col xs={12} md={10}>
                      <Col className="sender-name mb-2">
                        <Link
                          className="link-unstyled"
                          to={`/users/${post.senderId}`}
                        >
                          {post.senderUsername}
                        </Link>
                      </Col>

                      <Col className="post-content mb-2 ">{post.content}</Col>
                    </Col>
                  </Row>
                </li>
              );
            })}
          </ul>
        )}
      </Col>
    </Row>
  );
}
