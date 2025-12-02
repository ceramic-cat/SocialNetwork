import { useState } from "react";
import CreatePostModal from "../components/CreatePostModal";

export default function Home() {
  const [show] = useState(true);

  return (
    <div className="text-center">
      {/* Change to the correct senderId and senderName when we have users */}
      <CreatePostModal senderId="user-123" senderName="Maja Berg" />
    </div>
  );
}
