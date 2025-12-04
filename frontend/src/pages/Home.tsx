import { useState, useEffect } from "react";
import CreatePostModal from "../components/CreatePostModal";
import AuthModal from "../modals/AuthModal";
import { Button } from "react-bootstrap";

export default function Home() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function checkAuthStatus() {
      const token = localStorage.getItem("token");

      if (!token) {
        setIsLoggedIn(false);
        setLoading(false);
        return;
      }

      try {
        const response = await fetch("/api/auth/validate", {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        });

        if (response.ok) {
          setIsLoggedIn(true);
        } else {
          localStorage.removeItem("token");
          setIsLoggedIn(false);
        }
      } catch (error) {
        localStorage.removeItem("token");
        setIsLoggedIn(false);
      }

      setLoading(false);
    }

    checkAuthStatus();
  }, []);

  const handleLoginSuccess = () => {
    setIsLoggedIn(true);
  };

  const handleLogout = () => {
    localStorage.removeItem("token");
    setIsLoggedIn(false);
  };

  if (loading) {
    return <div className="text-center">Loading...</div>;
  }

  if (!isLoggedIn) {
    return (
      <AuthModal showInitially={true} onLoginSuccess={handleLoginSuccess} />
    );
  }

  return (
    <div className="text-center">
      {/* Change to the correct senderId and senderName when we have users */}
      <CreatePostModal
        senderId="405F3E28-E455-4F15-95E8-A890F54C5848"
        senderName="Maja Berg"
      />
      <Button variant="outline-danger" onClick={handleLogout}>
        Logout
      </Button>
    </div>
  );
}
