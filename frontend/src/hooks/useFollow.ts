import { useEffect, useState } from "react";
import { API } from "../config/api";

export function useFollow(userId: string | undefined, skip: boolean = false) {
  const [isFollowing, setIsFollowing] = useState(false);
  const [isLoading, setIsLoading] = useState(!skip);
  const [error, setError] = useState<string | null>(null);
  const [isUpdating, setIsUpdating] = useState(false);

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

  const toggleFollow = async () => {
    if (!userId || isUpdating) return;

    setIsUpdating(true);
    setError(null);
    const token = localStorage.getItem("token");

    try {
      const response = await fetch(API.FOLLOW.FOLLOW(userId), {
        method: isFollowing ? "DELETE" : "POST",
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-type": "application/json",
        },
      });
      if (!response.ok) {
        throw new Error("Failed to update follow status");
      }

      setIsFollowing(() => !isFollowing);
    } catch (error) {
      setError("Unable to update follow status");
    } finally {
      setIsUpdating(false);
    }
  };

  return { isLoading, error, isFollowing, isUpdating, toggleFollow };
}
