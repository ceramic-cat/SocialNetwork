import type { ReactNode } from "react";
import { useParams } from "react-router-dom";
import { Col, Row } from "react-bootstrap";
import { useDeletePost } from "../hooks/useDeletePost";
import useCurrentUser from "../hooks/useCurrentUser";
import PostCard from "./PostCard";
import TimelineStatus from "./TimelineStatus";
import { useTimeline } from "../hooks/useTimeline";
import { Link } from "react-router-dom";

export default function Timeline() {
  const { id: userId } = useParams<{ id: string }>();
  const { posts, isLoading, error, setPosts } = useTimeline(userId);
  const { userId: currentUserId } = useCurrentUser();
  const { deletePost, loading: deleting, error: deleteError } = useDeletePost();

  const timelineOwnerName =
    posts.length > 0 && posts[0].receiverUsername
      ? posts[0].receiverUsername
      : "Timeline";

  let content: ReactNode;

  async function handleDelete(postId: string) {
    const confirmed = window.confirm(
      "Are you sure you want to delete this post?"
    );
    if (!confirmed) return;

    const ok = await deletePost(postId);
    if (ok) {
      setPosts((prev) => prev.filter((p) => p.id !== postId));
    }
  }

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
            sender={
              <Link
                className="link-unstyled"
                to={`/users/${post.senderId}`}
              >
                {post.senderUsername}
              </Link>
            }
            content={post.content}
            timestamp={new Date(post.createdAt).toLocaleString()}
            canDelete={currentUserId === post.senderId}
            onDelete={() => handleDelete(post.id)}
            deleting={deleting}
          />
        ))}
      </>
    );
  }
  return (
    <Row className="justify-content-center">
      <Col xs={12}>
        <div>
          {deleteError && (
            <div className="text-danger mb-2 small">{deleteError}</div>
          )}
          <h2 className="feed-title">
            {posts.length === 0
              ? "Empty Timeline"
              : `${timelineOwnerName}'s Timeline`}
          </h2>
        </div>
      </Col>
      <Col xs={12} md={8} lg={6} className="feed-list-container">
        <div className="timeline-content">{content}</div>
      </Col>
    </Row>
  );
}
