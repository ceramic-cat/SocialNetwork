import { useEffect, useState } from "react";
import { API } from "../config/api";

import { type FollowsInfo } from "../types/FollowsInfo";

export function useGetFollowsInfo() {
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [follows, setFollows] = useState<FollowsInfo[]>([]);

  useEffect(() => {
    const loadFollows = async () => {
      setIsLoading(true);
      setError(null);

      const token = localStorage.getItem("token");

      try {
        const response = await fetch(API.FOLLOW.GETFOLLOWSINFO, {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        });
        if (!response.ok) {
          throw new Error("Failed to load relationship");
        }
        const data: FollowsInfo[] = await response.json();
        setFollows(data);
      } catch (error) {
        setError("Unable to load relationship");
        setFollows([]);
      } finally {
        setIsLoading(false);
      }
    };

    loadFollows();
  }, []);

  return { isLoading, error, follows };
}
