import { useEffect, useState } from "react";
import { API } from "../config/api";
import { type FollowsInfo } from "../types/FollowsInfo";

export function useGetFollowersInfo() {
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [followers, setFollowers] = useState<FollowsInfo[]>([]);

  useEffect(() => {
    const loadFollowers = async () => {
      setIsLoading(true);
      setError(null);

      const token = localStorage.getItem("token");

      try {
        const response = await fetch(API.FOLLOW.GETFOLLOWERSINFO, {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        });
        if (!response.ok) {
          throw new Error("Failed to load followers");
        }
        const data: FollowsInfo[] = await response.json();
        setFollowers(data);
      } catch (error) {
        setError("Unable to load followers");
        setFollowers([]);
      } finally {
        setIsLoading(false);
      }
    };

    loadFollowers();
  }, []);

  return { isLoading, error, followers };
}
