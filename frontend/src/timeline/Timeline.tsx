import { useEffect } from "react";
import { useState } from "react";
import PostCard from "./PostCard";

type PostDto = {
  id: string;
  senderId: string;
  recieverId: string;
  content: string;
  createdAt: string;
};

const BASE_URL = "http://localhost:5148";

export default function Timeline() {
  const userId = "userID";

  const [posts, setPosts] = useState<PostDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setIsLoading(true);
    setError(null);

    fetch(`${BASE_URL}/api/users/${userId}/timeline`)
      .then((res) => res.json())
      .then((data: PostDto[]) => {
        console.log("Timeline data:", data);
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
  }, []);

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
      Timeline Page - Under Construction. Try looking at the console.log
      {posts.map((post) => (
        <PostCard
          key={post.id}
          sender={post.senderId}
          content={post.content}
          timestamp={new Date(post.createdAt).toLocaleString()}
        />
      ))}
    </div>
  );
}
