import { useEffect, useState } from "react";
import { API } from "../config/api";

export function useGetUserFollowStats(userId: string) {
  const [stats, setStats] = useState<{ followers: number; following: number }>({
    followers: 0,
    following: 0,
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!userId) return;
    setLoading(true);
    setError(null);

    const token = localStorage.getItem("token");

    fetch(API.FOLLOW.GETUSERSTATS(userId), {
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    })
      .then((res) => (res.ok ? res.json() : Promise.reject()))
      .then((data) => setStats(data))
      .catch(() => setError("Failed to load stats"))
      .finally(() => setLoading(false));
  }, [userId]);

  return { stats, loading, error };
}
