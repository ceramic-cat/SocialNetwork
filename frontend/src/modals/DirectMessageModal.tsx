import { Modal, Button } from "react-bootstrap";
import CreateDirectMessageForm from "../forms/CreateDirectMessageForm";

export interface DirectMessageModalProps {
  show: boolean;
  onClose: () => void;
  receiverId?: string;
}

export default function DirectMessageModal({
  show,
  onClose,
  receiverId,
}: DirectMessageModalProps) {
  return (
    <Modal 
      show={show} 
      onHide={onClose} 
      centered
      backdrop="static"
      keyboard={true}
    >
      <Modal.Body>
        <CreateDirectMessageForm receiverId={receiverId} onSuccess={onClose} />
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={onClose}>
          Close
        </Button>
      </Modal.Footer>
    </Modal>
  );
}
