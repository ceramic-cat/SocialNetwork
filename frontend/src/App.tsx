import { Route, Routes } from "react-router-dom";
import { useState } from "react";
import TheFeed from "./pages/TheFeed";
import Header from "./partials/Header";
import useCurrentUser from "./hooks/useCurrentUser";
import AuthModal from "./modals/AuthModal";
import CreatePostModal from "./modals/CreatePostModal";
import DirectMessageModal from "./modals/DirectMessageModal";
import User from "./pages/User";
import SearchPage from "./pages/Search";
import NotFound from "./pages/NotFound";

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
  const [showMessageModal, setShowMessageModal] = useState(false);
  const [messageReceiverId, setMessageReceiverId] = useState<string | undefined>(undefined);

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
          onSendMessage={() => {
            setMessageReceiverId(undefined);
            setShowMessageModal(true);
          }}
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
              <Route path="/search" element={<SearchPage />} />
              <Route 
                path="/users/:id" 
                element={
                  <User 
                    onSendMessage={(receiverId) => {
                      setMessageReceiverId(receiverId);
                      setShowMessageModal(true);
                    }}
                  />
                } 
              />
              <Route path="*" element={<NotFound />} />
            </Routes>

            {userId && username && (
              <>
                <CreatePostModal
                  senderId={userId}
                  senderName={username}
                  show={showCreatePost}
                  onClose={() => setShowCreatePost(false)}
                />
                <DirectMessageModal
                  show={showMessageModal}
                  onClose={() => {
                    setShowMessageModal(false);
                    setMessageReceiverId(undefined);
                  }}
                  receiverId={messageReceiverId}
                />
              </>
            )}
          </>
        )}
      </main>
    </div>
  );
}

export default App;
