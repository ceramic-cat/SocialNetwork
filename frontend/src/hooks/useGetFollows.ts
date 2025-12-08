import { useEffect, useState } from "react";
import { API } from "../config/api";

export function useGetFollows() {
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [follows, setFollows] = useState();

  useEffect(() => {
    const loadFollows = async () => {
      setIsLoading(true);
      setError(null);

      const token = localStorage.getItem("token");

      try {
        const response = await fetch(API.FOLLOW.GETFOLLOWS, {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        });
        if (!response.ok) {
          throw new Error("Failed to load relationship");
        }
        const data = await response.json();
        setFollows(data);
      } catch (error) {
        setError("Unable to load relationship");
        // setFollows();
      } finally {
        setIsLoading(false);
      }
    };

    loadFollows();
  }, []);

  return { isLoading, error, follows };
}
