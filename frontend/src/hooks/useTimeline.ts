import { useEffect, useState, type Dispatch, type SetStateAction } from "react";
import { API } from "../config/api";
import type { PostDto } from "../types/PostDto";

type UseTimelineResult = {
  posts: PostDto[];
  isLoading: boolean;
  error: string | null;
  setPosts: Dispatch<SetStateAction<PostDto[]>>;
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
        const res = await fetch(API.USERS.TIMELINE(userId));

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

  return { posts, isLoading, error, setPosts };
}
