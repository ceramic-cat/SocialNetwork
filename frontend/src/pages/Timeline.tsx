import { useEffect } from "react";

type PostDto = {
  id: string;
  senderId: string;
  reciverId: string;
  content: string;
  createdAt: string;
};

const BASE_URL = "http://localhost:5148";

export default function Timeline() {
  const userId = "This works with a hardcoded user id";

  useEffect(() => {
    fetch(`${BASE_URL}/api/users/${userId}/timeline`)
      .then((res) => res.json())
      .then((data: PostDto[]) => {
        console.log("Timeline data:", data);
      })
      .catch((err) => console.error("Failed to load timeline:", err));
  }, []);
  return (
    <div>
      Timeline Page - Under Construction. Try looking at the console.log
    </div>
  );
}
