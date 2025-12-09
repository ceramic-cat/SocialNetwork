import { Accordion, Col, Container, Row, Button } from "react-bootstrap";
import FollowsList from "./FollowsList";
import FollowersList from "./FollowerList";
import { useGetUserFollowStats } from "../hooks/useGetUserFollowStats";
import useCurrentUser from "../hooks/useCurrentUser";

interface OwnUserProfileProps {
  username: string | null;
}

export default function OwnUserProfile({ username }: OwnUserProfileProps) {
  const { userId } = useCurrentUser();
  const { stats, loading, error } = useGetUserFollowStats(userId || "");

  function handleEdit() {
    // Show edit modal or navigate to edit page
  }

  function handleDelete() {
    // Show delete confirmation or perform delete
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
    </Container>
  );
}
