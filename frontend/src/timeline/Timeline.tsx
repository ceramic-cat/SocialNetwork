import React, { ReactNode } from "react";
import { useEffect } from "react";
import { useState } from "react";
import PostCard from "./PostCard";
import TimelineStatus from "./TimelineStatus";
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
    if (!userId) return;
    setIsLoading(true);
    setError(null);

    fetch(`${BASE_URL}/api/users/${userId}/timeline`)
      .then((res) => {
        if (!res.ok) {
          throw new Error("Failed to load timeline");
        }
        return res.json();
      })
      .then((data: PostDto[]) => {
        setPosts(data);
      })
      .catch(() => {
        setError("Could not load timeline.");
        setPosts([]);
      })
      .finally(() => {
        setIsLoading(false);
      });
  }, [userId]);
  let content: ReactNode;

  if (isLoading) {
    content = <TimelineStatus message="Loading timeline…" type="loading" />;
  } else if (error) {
    content = <TimelineStatus message={error} type="error" />;
  } else if (posts.length === 0) {
    content = (
      <TimelineStatus message="No posts to display in this timeline." />
    );
  } else {
    content = (
      <>
        {posts.map((post) => (
          <PostCard
            key={post.id}
            sender={post.senderId}
            content={post.content}
            timestamp={new Date(post.createdAt).toLocaleString()}
          />
        ))}
      </>
    );
  }

  return (
    <Row className="justify-content-center">
      <Col xs={12}>
        <h2 className="feed-title">{userId} Timeline</h2>
      </Col>
      <Col xs={12} md={8} lg={6} className="feed-list-container">
        <div className="timeline-content">{content}</div>
      </Col>
    </Row>
  );
}
