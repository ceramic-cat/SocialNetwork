import { Route, Routes } from "react-router-dom";
import { useState } from "react";
import TheFeed from "./pages/TheFeed";
import Timeline from "./timeline/Timeline";
import Header from "./partials/Header";
import useCurrentUser from "./hooks/useCurrentUser";
import AuthModal from "./modals/AuthModal";
import CreatePostModal from "./modals/CreatePostModal";
import User from "./pages/User";

function App() {
  const {
    isLoggedIn,
    loading,
    userId,
    username,
    handleLoginSuccess,
    handleLogout,
  } = useCurrentUser();

  const [showCreatePost, setShowCreatePost] = useState(false);
  const [showMessageModal, setShowMessageModal] = useState(false); // when we have a message modal

  if (loading) {
    return (
      <div className="app-shell d-flex align-items-center justify-content-center">
        Loading...
      </div>
    );
  }

  return (
    <div className="page-container">
      {isLoggedIn && (
        <Header
          isLoggedIn={isLoggedIn}
          username={username}
          userId={userId}
          onCreatePost={() => setShowCreatePost(true)}
          onSendMessage={() => setShowMessageModal(true)}
          onLogout={handleLogout}
        />
      )}

      <main className="page-background">
        {!isLoggedIn ? (
          <AuthModal showInitially={true} onLoginSuccess={handleLoginSuccess} />
        ) : (
          <>
            <Routes>
              <Route path="/" element={<TheFeed />} />
              <Route path="/users/:id/timeline" element={<Timeline />} />
              <Route path="/users/:id" element={<User />} />
            </Routes>

            {userId && username && (
              <CreatePostModal
                senderId={userId}
                senderName={username}
                show={showCreatePost}
                onClose={() => setShowCreatePost(false)}
              />
            )}
          </>
        )}
      </main>
    </div>
  );
}

export default App;
