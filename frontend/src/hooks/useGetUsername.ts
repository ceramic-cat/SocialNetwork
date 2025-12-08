import { useState, useEffect } from "react";
import { API } from "../config/api";

export default function useGetUsername(userId: string | undefined) {
  const [username, setUsername] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!userId) return;

    setIsLoading(true);
    setError(null);
    const token = localStorage.getItem("token");

    const loadUsername = async () => {
      try {
        const result = await fetch(API.AUTH.GET_USERNAME(userId), {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        });

        if (!result.ok) {
          throw new Error("Failed to load username");
        }
        const data: string = await result.json();
        setUsername(data);
      } catch (error) {
        setError("Unable to load username");
      } finally {
        setIsLoading(false);
      }
    };

    loadUsername();
  }, [userId]);

  return { isLoading, error, username };
}
