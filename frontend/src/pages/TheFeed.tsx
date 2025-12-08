import { useEffect, useState } from "react";
import { Row, Col, Button } from "react-bootstrap";
import { Link } from "react-router-dom";
import { useNavigate } from "react-router-dom";
import useCurrentUser from "../hooks/useCurrentUser";

type PostDto = {
  id: string;
  senderId: string;
  senderUsername: string;
  receiverId: string;
  content: string;
  createdAt: string;
};

export default function TheFeed() {
  const [posts, setPosts] = useState<PostDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [following, setFollowing] = useState<string[]>([]);

  const { userId, loading: userLoading, isLoggedIn } = useCurrentUser();

  const navigate = useNavigate();

  useEffect(() => {
    if (!userId || userLoading) return;

    async function fetchFeed() {
      setLoading(true);
      setError(null);
      try {
        const res = await fetch(`http://localhost:5148/api/thefeed/${userId}`);
        if (res.ok) {
          const data = await res.json();
          setPosts(data);
        } else {
          setError("Failed to load feed.");
        }
      } catch (err) {
        setError("Network error.");
      }
      setLoading(false);
    }
    fetchFeed();
  }, [userId, userLoading]);

  useEffect(() => {
    if (!userId || userLoading) return;

    async function fetchFollowing() {
      const token = localStorage.getItem("token");
      if (!token) return;
      try {
        const res = await fetch("http://localhost:5148/api/follow", {
          headers: { Authorization: `Bearer ${token}` },
        });
        if (res.ok) {
          const data = await res.json();
          setFollowing(data);
        }
      } catch {
        setError("Network error.");
      }
    }
    fetchFollowing();
  }, [userId, userLoading]);
  if (userLoading) return <div>Loading user...</div>;
  if (!isLoggedIn) return <div></div>;

  return (
    <Row className="justify-content-center text-center">
      <div className="feed-space-animation"></div>

      <Col xs={12}>
        <h2 className="feed-title">Your Feed</h2>
      </Col>
      <Col xs={12} md={8} lg={6} className="feed-list-container">
        {loading ? (
          <Col className="alert-info">Loading...</Col>
        ) : error ? (
          <Col className="alert-danger">{error}</Col>
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
            {posts.map((post) => (
              <li key={post.id} className="feed-post">
                <Row>
                  <Col xs={12} md={10}>
                    <Col className="sender-name mb-2">
                      <Link
                        className="link-unstyled"
                        to={`/users/${post.senderId}/timeline`}
                      >
                        {post.senderUsername}
                      </Link>
                    </Col>
                    <Col className="post-content mb-2 ">{post.content}</Col>
                    <Col className="post-date mt-2">
                      <i className="bi bi-clock-history" />{" "}
                      {new Date(post.createdAt).toLocaleString()}
                    </Col>
                  </Col>
                </Row>
              </li>
            ))}
          </ul>
        )}
      </Col>
    </Row>
  );
}
