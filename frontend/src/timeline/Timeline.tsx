import type { ReactNode } from "react";
import { useParams } from "react-router-dom";
import { Col, Row } from "react-bootstrap";

import PostCard from "./PostCard";
import TimelineStatus from "./TimelineStatus";
import { useTimeline } from "../hooks/useTimeline";

export default function Timeline() {
  const { id: userId } = useParams<{ id: string }>();

  const { posts, isLoading, error } = useTimeline(userId);

  const timelineOwnerName =
    posts.length > 0 && posts[0].receiverUsername
      ? posts[0].receiverUsername
      : userId ?? "Timeline";

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
            sender={post.senderUsername}
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
        <h2 className="feed-title">
          {posts.length === 0
            ? "Empty Timeline"
            : `${timelineOwnerName}'s Timeline`}
        </h2>
      </Col>
      <Col xs={12} md={8} lg={6} className="feed-list-container">
        <div className="timeline-content">{content}</div>
      </Col>
    </Row>
  );
}
