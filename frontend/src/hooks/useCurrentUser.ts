import { useState, useEffect } from "react";
import { API } from "../config/api";

interface CurrentUser {
  id: string;
  username: string;
}
const DELETE_ACCOUNT_ENDPOINT = `${BASE_URL}/api/auth/delete-account`;

export default function useCurrentUser() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [loading, setLoading] = useState(true);
  const [user, setUser] = useState<CurrentUser | null>(null);

  useEffect(() => {
    async function checkAuthStatus() {
      const token = localStorage.getItem("token");

      if (!token) {
        setIsLoggedIn(false);
        setUser(null);
        setLoading(false);
        return;
      }

      try {
        const response = await fetch(API.AUTH.VALIDATE, {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        });

        if (response.ok) {
          const data = await response.json();

          setIsLoggedIn(true);
          setUser({
            id: data.userId,
            username: data.username,
          });
        } else {
          localStorage.removeItem("token");
          setIsLoggedIn(false);
          setUser(null);
        }
      } catch {
        localStorage.removeItem("token");
        setIsLoggedIn(false);
        setUser(null);
      }

      setLoading(false);
    }

    checkAuthStatus();
  }, []);

  const handleLoginSuccess = async () => {
    const token = localStorage.getItem("token");
    if (!token) return;

    try {
      const res = await fetch(API.AUTH.VALIDATE, {
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
      });

      if (res.ok) {
        const data = await res.json();

        setIsLoggedIn(true);
        setUser({
          id: data.userId,
          username: data.username,
        });
      }
    } catch {}
  };

  const handleLogout = () => {
    localStorage.removeItem("token");
    setIsLoggedIn(false);
    setUser(null);
  };

  const handleDeleteAccount = async () => {
    const token = localStorage.getItem("token");
    if (!token) return;

    try {
      const response = await fetch(DELETE_ACCOUNT_ENDPOINT, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
      });

      if (response.ok) {
        localStorage.removeItem("token");
        setIsLoggedIn(false);
        setUser(null);
      }
    } catch {}
  };

  return {
    isLoggedIn,
    loading,
    user,
    userId: user?.id ?? null,
    username: user?.username ?? null,
    handleLoginSuccess,
    handleLogout,
    handleDeleteAccount,
  };
}
