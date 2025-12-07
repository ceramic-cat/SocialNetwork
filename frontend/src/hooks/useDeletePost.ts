import { useState } from "react";
import { API } from "../config/api";

export function useDeletePost() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function deletePost(postId: string): Promise<boolean> {
    setLoading(true);
    setError(null);

    const token = localStorage.getItem("token");

    try {
      const response = await fetch(`${API.POSTS}/${postId}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        const text = await response.text();
        setError(text || "Failed to delete post.");
        return false;
      }

      return true;
    } catch {
      setError("Network error while deleting post.");
      return false;
    } finally {
      setLoading(false);
    }
  }

  return { deletePost, loading, error };
}
