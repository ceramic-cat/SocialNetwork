import { useEffect } from "react";
import { Card } from "react-bootstrap";
import { useState } from "react";

type PostDto = {
  id: string;
  senderId: string;
  recieverId: string;
  content: string;
  createdAt: string;
};

const BASE_URL = "http://localhost:5148";

export default function Timeline() {
  const userId = "Works if userid is added hardcoded";

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
      <Card className="mt-5">
        <Card.Header>Quote</Card.Header>
        <Card.Body>
          <blockquote className="blockquote mb-0">
            <ul>
              {posts.map((post) => (
                <li key={post.id}>{post.content}</li>
              ))}
            </ul>
            <footer className="blockquote-footer">
              Someone famous in <cite title="Source Title">Source Title</cite>
            </footer>
          </blockquote>
        </Card.Body>
      </Card>
      ;
    </div>
  );
}
