import { useEffect, useState } from "react";
import type { PostDto } from "../types/PostDto";

export function useFeedPosts(userId: string | null, userLoading: boolean) {
  const [posts, setPosts] = useState<PostDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

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

  return { posts, loading, error };
}
