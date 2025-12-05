import { Modal, Button } from "react-bootstrap";
import CreatePostForm from "../forms/CreatePostForm";

export interface CreatePostModalProps {
  senderId: string;
  senderName: string;
  show: boolean;
  onClose: () => void;
}

export default function CreatePostModal({
  senderId,
  senderName,
  show,
  onClose,
}: CreatePostModalProps) {
  return (
    <Modal show={show} onHide={onClose} centered>
      <Modal.Body>
        <CreatePostForm
          senderId={senderId}
          senderName={senderName}
          onSuccess={onClose}
        />
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={onClose}>
          Close
        </Button>
      </Modal.Footer>
    </Modal>
  );
}
