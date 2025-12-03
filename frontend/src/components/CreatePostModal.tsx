import { useState } from "react";
import { Button, Modal } from "react-bootstrap";
import CreatePostForm from "./CreatePostForm";

export interface CreatePostModalProps {
  senderId: string;
  senderName: string;
}

export default function CreatePostModal({
  senderId,
  senderName,
}: CreatePostModalProps) {
  const [show, setShow] = useState(false);

  function handleOpen() {
    setShow(true);
  }

  function handleClose() {
    setShow(false);
  }

  return (
    <>
      <Button variant="primary" onClick={handleOpen}>
        Create a post
      </Button>

      <Modal show={show} onHide={handleClose} centered>
        <Modal.Body>
          <CreatePostForm
            senderId={senderId}
            senderName={senderName}
            onSuccess={handleClose}
          />
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={handleClose}>
            Close
          </Button>
        </Modal.Footer>
      </Modal>
    </>
  );
}
