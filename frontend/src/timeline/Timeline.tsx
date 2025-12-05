import { useEffect } from "react";
import { useState } from "react";
import PostCard from "./PostCard";

type PostDto = {
  id: string;
  senderId: string;
  recieverId: string;
  content: string;
  createdAt: string;
};

const BASE_URL = "http://localhost:5148";

export default function Timeline() {
  const userId = "405F3E28-E455-4F15-95E8-A890F54C5848";

  const [posts, setPosts] = useState<PostDto[]>([]);

  useEffect(() => {
    fetch(`${BASE_URL}/api/users/${userId}/timeline`)
      .then((res) => res.json())
      .then((data: PostDto[]) => {
        console.log("Timeline data:", data);
        setPosts(data);
      })
      .catch((err) => console.error("Failed to load timeline:", err));
  }, []);

  return (
    <div>
      Timeline Page - Under Construction. Try looking at the console.log
      {posts.map((post) => (
        <PostCard
          key={post.id}
          sender={post.senderId}
          content={post.content}
          timestamp={new Date(post.createdAt).toLocaleString()}
        />
      ))}
    </div>
  );
}
