import { useEffect, useState } from "react";
import { API } from "../config/api";

export function useIsFollowing(
  userId: string | undefined,
  skip: boolean = false
) {
  const [isFollowing, setIsFollowing] = useState(false);
  const [isLoading, setIsLoading] = useState(!skip);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!userId || skip) {
      setIsLoading(false);
      return;
    }

    const loadIsFollowing = async () => {
      setIsLoading(true);
      setError(null);

      const token = localStorage.getItem("token");

      try {
        const response = await fetch(API.FOLLOW.FOLLOW(userId), {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        });
        if (!response.ok) {
          throw new Error("Failed to load relationship");
        }
        const data: boolean = await response.json();
        setIsFollowing(data);
      } catch (error) {
        setError("Unable to load relationship");
        setIsFollowing(false);
      } finally {
        setIsLoading(false);
      }
    };

    loadIsFollowing();
  }, [userId, skip]);

  return { isLoading, error, isFollowing };
}
