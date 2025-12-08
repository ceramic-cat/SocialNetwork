// src/hooks/useUserSearch.ts
import { useState } from "react";
import { API } from "../config/api";

export interface UserSearchResult {
  id: string;
  username: string;
}

interface UseUserSearchResult {
  results: UserSearchResult[];
  loading: boolean;
  error: string | null;
  search: (query: string) => Promise<void>;
  clear: () => void;
}

export function useUserSearch(): UseUserSearchResult {
  const [results, setResults] = useState<UserSearchResult[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function search(query: string): Promise<void> {
    setError(null);

    const trimmed = query.trim();

    if (!trimmed) {
      setResults([]);
      return;
    }

    if (trimmed.length < 2) {
      setResults([]);
      return;
    }

    setLoading(true);

    try {
      const res = await fetch(API.USERS.SEARCH(trimmed), {
        headers: {
          "Content-Type": "application/json",
        },
      });

      if (!res.ok) {
        const text = await res.text();
        setError(text || "Failed to search users.");
        setResults([]);
        return;
      }

      const data: UserSearchResult[] = await res.json();
      setResults(data);
    } catch {
      setError("Network error while searching users.");
      setResults([]);
    } finally {
      setLoading(false);
    }
  }

  function clear() {
    setResults([]);
    setError(null);
  }

  return { results, loading, error, search, clear };
}
