import { useState } from "react";
import type { CreatePostRequest } from "../types/post";

export interface UseCreatePostResult {
  createPost: (payload: CreatePostRequest) => Promise<boolean>;
  loading: boolean;
  error: string | null;
  success: string | null;
  resetStatus: () => void;
}

const BASE_URL = "http://localhost:5148";
const ENDPOINT = `${BASE_URL}/api/posts`;

export function useCreatePost(): UseCreatePostResult {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  async function createPost(payload: CreatePostRequest): Promise<boolean> {
    setError(null);
    setSuccess(null);
    setLoading(true);

    try {
      const token = localStorage.getItem("token");

      const headers: HeadersInit = {
        "Content-Type": "application/json",
      };

      if (token) {
        headers["Authorization"] = `Bearer ${token}`;
      }

      const response = await fetch(ENDPOINT, {
        method: "POST",
        headers,
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
