import { useEffect } from "react";
import { useState } from "react";
import PostCard from "./PostCard";

import { useParams } from "react-router-dom";
import { Col, Row } from "react-bootstrap";

type PostDto = {
  id: string;
  senderId: string;
  recieverId: string;
  content: string;
  createdAt: string;
};

const BASE_URL = "http://localhost:5148";

export default function Timeline() {
  const { id: userId } = useParams<{ id: string }>();

  const [posts, setPosts] = useState<PostDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setIsLoading(true);
    setError(null);

    fetch(`${BASE_URL}/api/users/${userId}/timeline`)
      .then((res) => res.json())
      .then((data: PostDto[]) => {
        setPosts(data);
      })
      .catch((err) => {
        setError("Could not load timeline.");
        setPosts([]);
      })
      .finally(() => {
        setIsLoading(false);
      });
  }, [userId]);

  if (isLoading) {
    return <div>Loading timeline…</div>;
  }

  if (error) {
    return <div>{error}</div>;
  }

  if (posts.length === 0) {
    return <div>No posts to display in this timeline.</div>;
  }

  return (
    <Row className="justify-content-center">
      <Col xs={12}>
        <h2 className="feed-title">{userId} Timeline</h2>
      </Col>
      <Col xs={12} md={8} lg={6} className="feed-list-container">
        <div>
          {posts.map((post) => (
            <PostCard
              key={post.id}
              sender={post.senderId}
              content={post.content}
              timestamp={new Date(post.createdAt).toLocaleString()}
            />
          ))}
        </div>
      </Col>
    </Row>
  );
}
