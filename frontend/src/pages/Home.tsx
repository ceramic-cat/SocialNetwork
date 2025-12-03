import { useState } from "react";
import CreatePostModal from "../components/CreatePostModal";

export default function Home() {
  const [show] = useState(true);

  return (
    <div className="text-center">
      {/* Change to the correct senderId and senderName when we have users */}
      <CreatePostModal
        senderId="405F3E28-E455-4F15-95E8-A890F54C5848"
        senderName="Maja Berg"
      />
    </div>
  );
}
