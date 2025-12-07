import { useEffect, useState } from "react";

const BASE_URL = "http://localhost:5148";

export type PostDto = {
  id: string;
  senderId: string;
  recieverId: string;
  content: string;
  createdAt: string;
};

type UseTimelineResult = {
  posts: PostDto[];
  isLoading: boolean;
  error: string | null;
};

export function useTimeline(userId: string | undefined): UseTimelineResult {
  const [posts, setPosts] = useState<PostDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!userId) return;

    const loadTimeline = async () => {
      setIsLoading(true);
      setError(null);

      try {
        const res = await fetch(`${BASE_URL}/api/users/${userId}/timeline`);

        if (!res.ok) {
          throw new Error("Failed to load timeline");
        }

        const data: PostDto[] = await res.json();
        setPosts(data);
      } catch (err) {
        setError("Could not load timeline.");
        setPosts([]);
      } finally {
        setIsLoading(false);
      }
    };

    loadTimeline();
  }, [userId]);

  return { posts, isLoading, error };
}
