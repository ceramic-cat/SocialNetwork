import { useState } from "react";

const API_URL = "http://localhost:5148/api/auth/edit-profile";

export function useEditProfile(onSuccess?: () => void) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  async function editProfile(
    field: "username" | "email" | "password",
    value: string
  ) {
    setError(null);
    setSuccess(null);
    setLoading(true);

    let body: any = {};
    if (field && value) body[field] = value;

    try {
      const token = localStorage.getItem("token");
      const response = await fetch(API_URL, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(body),
      });

      if (!response.ok) {
        const data = await response.text();
        setError(data || "Could not update profile.");
      } else {
        setSuccess("Profile updated successfully!");
        if (onSuccess) onSuccess();
      }
    } catch (err) {
      setError("An error occurred. Please try again.");
    } finally {
      setLoading(false);
    }
  }

  function resetMessages() {
    setError(null);
    setSuccess(null);
  }

  return { loading, error, success, editProfile, resetMessages };
}
