import type { ReactNode } from "react";
import { useEffect } from "react";
import { useState } from "react";
import PostCard from "./PostCard";
import TimelineStatus from "./TimelineStatus";
import { useParams } from "react-router-dom";
import { Col, Row } from "react-bootstrap";
import { useDeletePost } from "../hooks/useDeletePost";
import useCurrentUser from "../hooks/useCurrentUser";
import { API } from "../config/api";

type PostDto = {
  id: string;
  senderId: string;
  recieverId: string;
  content: string;
  createdAt: string;
};

export default function Timeline() {
  const { id: userId } = useParams<{ id: string }>();

  const [posts, setPosts] = useState<PostDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const { userId: currentUserId } = useCurrentUser();
  const { deletePost, loading: deleting, error: deleteError } = useDeletePost();

  useEffect(() => {
    if (!userId) {
      setError("No user id provided.");
      setIsLoading(false);
      return;
    }

    setIsLoading(true);
    setError(null);

    fetch(API.USERS.TIMELINE(userId))
      .then((res) => {
        if (!res.ok) {
          throw new Error("Failed to load timeline");
        }
        return res.json();
      })
      .then((data) => {
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
            sender={post.senderId}
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
          <h2 className="feed-title">{userId} Timeline</h2>
        </div>
      </Col>
      <Col xs={12} md={8} lg={6} className="feed-list-container">
        <div className="timeline-content">{content}</div>
      </Col>
    </Row>
  );
}
