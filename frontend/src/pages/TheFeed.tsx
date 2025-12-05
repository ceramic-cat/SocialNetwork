import { useEffect, useState } from "react";
import useCurrentUser from "../hooks/useCurrentUser";

type PostDto = {
  id: string;
  senderId: string;
  senderUsername: string;
  receiverId: string;
  content: string;
  createdAt: string;
};

export default function TheFeed() {
  const [posts, setPosts] = useState<PostDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const { userId, loading: userLoading, isLoggedIn } = useCurrentUser();

  useEffect(() => {
    if (!userId || userLoading) return;

    async function fetchFeed() {
      setLoading(true);
      setError(null);
      try {
        const res = await fetch(`http://localhost:5148/api/thefeed/${userId}`);
        if (res.ok) {
          const data = await res.json();
          setPosts(data);
        } else {
          setError("Failed to load feed.");
        }
      } catch (err) {
        setError("Network error.");
      }
      setLoading(false);
    }
    fetchFeed();
  }, [userId, userLoading]);

  if (userLoading) return <div>Loading user...</div>;
  if (!isLoggedIn) return <div></div>;

  return (
    <div className="text-center">
      <h2 className="mb-4" style={{ color: "#b8b8ff" }}>
        Your Feed
      </h2>
      {loading ? (
        <div>Loading...</div>
      ) : error ? (
        <div style={{ color: "red" }}>{error}</div>
      ) : posts.length === 0 ? (
        <div>No posts to show.</div>
      ) : (
        <ul className="feed-list">
          {posts.map((post) => (
            <li key={post.id} className="feed-post">
              <div className="sender-name">{post.senderUsername}</div>
              <div className="post-content">{post.content}</div>
              <div className="post-date">
                <i className="bi bi-clock-history" />{" "}
                {new Date(post.createdAt).toLocaleString()}
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
