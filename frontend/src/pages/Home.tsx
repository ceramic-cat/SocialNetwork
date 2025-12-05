import { Button } from "react-bootstrap";
import CreatePostModal from "../modals/CreatePostModal";
import AuthModal from "../modals/AuthModal";
import useCurrentUser from "../hooks/useCurrentUser";

export default function Home() {
  const {
    isLoggedIn,
    loading,
    userId,
    username,
    handleLoginSuccess,
    handleLogout,
  } = useCurrentUser();

  if (loading) {
    return <div className="text-center">Loading...</div>;
  }

  if (!isLoggedIn) {
    return (
      <div className="auth-page-background">
        <AuthModal showInitially={true} onLoginSuccess={handleLoginSuccess} />
      </div>
    );
  }

  return (
    <div className="text-center">
      {userId && username && (
        <CreatePostModal senderId={userId} senderName={username} />
      )}

      <Button variant="outline-danger" onClick={handleLogout}>
        Logout
      </Button>
    </div>
  );
}
