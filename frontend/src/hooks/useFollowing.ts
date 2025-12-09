import { useEffect, useState } from "react";

export function useFollowing(userId: string | null, userLoading: boolean) {
  const [following, setFollowing] = useState<string[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!userId || userLoading) return;

    async function fetchFollowing() {
      const token = localStorage.getItem("token");
      if (!token) return;
      try {
        const res = await fetch("http://localhost:5148/api/follow", {
          headers: { Authorization: `Bearer ${token}` },
        });
        if (res.ok) {
          const data = await res.json();
          setFollowing(data);
        }
      } catch {
        setError("Network error.");
      }
    }
    fetchFollowing();
  }, [userId, userLoading]);

  return { following, error };
}
