import { useEffect } from "react";
import { useState } from "react";
import PostCard from "./PostCard";
import { useParams } from "react-router-dom";
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
      .then((res) => res.json())
      .then((data: PostDto[]) => {
        setPosts(data);
      })
      .catch((err) => {
        console.error("Failed to load timeline:", err);
        setError("Could not load timeline.");
        setPosts([]);
      })
      .finally(() => {
        setIsLoading(false);
      });
  }, [userId]);

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
    return <div>Loading timeline…</div>;
  }

  if (error) {
    return <div>{error}</div>;
  }

  if (posts.length === 0) {
    return <div>No posts to display in this timeline.</div>;
  }

  return (
    <div>
      {deleteError && (
        <div className="text-danger mb-2 small">{deleteError}</div>
      )}

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
    </div>
  );
}
