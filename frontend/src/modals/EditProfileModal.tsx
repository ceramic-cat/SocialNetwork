import { Modal } from "react-bootstrap";
import EditProfileForm from "../forms/EditProfileForm";

interface EditProfileModalProps {
  show: boolean;
  onHide: () => void;
}

export default function EditProfileModal({
  show,
  onHide,
}: EditProfileModalProps) {
  return (
    <Modal show={show} onHide={onHide} centered className="edit-profile-modal">
      <Modal.Header closeButton>
        <Modal.Title>Edit Profile</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <EditProfileForm onSuccess={onHide} />
      </Modal.Body>
    </Modal>
  );
}
