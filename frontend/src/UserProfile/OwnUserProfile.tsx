import { Accordion, Col, Container, Row, Button } from "react-bootstrap";
import { useState } from "react";
import FollowsList from "./FollowsList";
import FollowersList from "./FollowerList";
import { useGetUserFollowStats } from "../hooks/useGetUserFollowStats";
import useCurrentUser from "../hooks/useCurrentUser";
import EditProfileModal from "../modals/EditProfileModal";
import ConfirmModal from "../modals/ConfirmModal";

interface OwnUserProfileProps {
  username: string | null;
}

export default function OwnUserProfile({ username }: OwnUserProfileProps) {
  const { userId, handleDeleteAccount } = useCurrentUser();
  const { stats, loading, error } = useGetUserFollowStats(userId || "");
  const [showEditModal, setShowEditModal] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [deleting, setDeleting] = useState(false);

  function handleEdit() {
    setShowEditModal(true);
  }

  function handleDelete() {
    setShowDeleteModal(true);
  }

  async function confirmDelete() {
    setDeleting(true);
    await handleDeleteAccount();
    setDeleting(false);
    setShowDeleteModal(false);
    window.location.reload();
  }

  return (
    <Container className="user-container position-relative">
      <Row className="justify-content-center">
        <Col xs={12}>
          <h1 className="user-header">Welcome back {username}</h1>
        </Col>
      </Row>
      <Row className="justify-content-center mt-3">
        <Col xs={12} md={5} lg={4}>
          <Accordion className="accordion-user">
            <Accordion.Item eventKey="0">
              <Accordion.Header>
                Your follows{" "}
                <span className="accordion-stat-number">
                  {loading ? "…" : error ? "!" : stats.following}
                </span>
              </Accordion.Header>
              <Accordion.Body>
                <FollowsList />
              </Accordion.Body>
            </Accordion.Item>
          </Accordion>
        </Col>
        <Col xs={12} md={5} lg={4}>
          <Accordion className="accordion-user">
            <Accordion.Item eventKey="1">
              <Accordion.Header>
                {" "}
                Your followers{" "}
                <span className="accordion-stat-number">
                  {loading ? "…" : error ? "!" : stats.followers}
                </span>
              </Accordion.Header>
              <Accordion.Body>
                <FollowersList />
              </Accordion.Body>
            </Accordion.Item>
          </Accordion>
        </Col>
      </Row>
      {/* Floating buttons */}
      <div className="floating-btn-row">
        <div className="floating-btn floating-btn-edit">
          <Button variant="secondary" onClick={handleEdit}>
            <i /> Edit profile
          </Button>
        </div>
        <div className="floating-btn floating-btn-delete">
          <Button variant="outline-danger" onClick={handleDelete}>
            <i /> Delete profile
          </Button>
        </div>
      </div>
      <EditProfileModal
        show={showEditModal}
        onHide={() => setShowEditModal(false)}
      />
      <ConfirmModal
        show={showDeleteModal}
        onHide={() => setShowDeleteModal(false)}
        onConfirm={confirmDelete}
        title="Delete Profile"
        message="Are you sure you want to delete your profile? This action cannot be undone."
        confirmText="Delete"
        confirmVariant="danger"
        loading={deleting}
      />
    </Container>
  );
}
