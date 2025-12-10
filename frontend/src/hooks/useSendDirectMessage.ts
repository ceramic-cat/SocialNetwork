import { useState } from "react";
import type { CreateDirectMessageRequest } from "../types/directMessage";
import { API } from "../config/api";

export interface UseSendDirectMessageResult {
  sendDirectMessage: (payload: CreateDirectMessageRequest) => Promise<boolean>;
  loading: boolean;
  error: string | null;
  success: string | null;
  resetStatus: () => void;
}

export function useSendDirectMessage(): UseSendDirectMessageResult {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  async function sendDirectMessage(
    payload: CreateDirectMessageRequest
  ): Promise<boolean> {
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

      const response = await fetch(API.DIRECT_MESSAGES, {
        method: "POST",
        headers,
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        try {
          const errorData = await response.json();
          const errorMessage =
            errorData?.errorMessage || "Failed to send direct message.";
          setError(errorMessage);
        } catch {
          const errorText = await response.text();
          setError(errorText || "Failed to send direct message.");
        }
        return false;
      }

      setSuccess("Direct message sent successfully!");
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

  return { sendDirectMessage, loading, error, success, resetStatus };
}
