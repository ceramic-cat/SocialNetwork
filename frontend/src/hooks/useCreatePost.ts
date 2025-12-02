import { useState } from "react";
import type { CreatePostRequest } from "../types/Post";

export interface UseCreatePostResult {
  createPost: (payload: CreatePostRequest) => Promise<boolean>;
  loading: boolean;
  error: string | null;
  success: string | null;
  resetStatus: () => void;
}

export function useCreatePost(): UseCreatePostResult {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  async function createPost(payload: CreatePostRequest): Promise<boolean> {
    setError(null);
    setSuccess(null);
    setLoading(true);

    try {
      const response = await fetch("/api/posts", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        const errorText = await response.text();
        setError(errorText || "Failed to create post.");
        return false;
      }

      setSuccess("Post created successfully!");
      return true;
    } catch {
      setError("Something went wrong. Please try again.");
      return false;
    } finally {
      setLoading(false);
    }
  }

  function resetStatus(): void {
    setError(null);
    setSuccess(null);
  }

  return { createPost, loading, error, success, resetStatus };
}
