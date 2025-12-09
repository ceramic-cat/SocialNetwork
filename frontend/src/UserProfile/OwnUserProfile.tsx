import { Accordion, Col, Container, Row } from "react-bootstrap";
import FollowsList from "./FollowsList";
import FollowersList from "./FollowerList";

interface OwnUserProfileProps {
  username: string | null;
}

export default function OwnUserProfile({ username }: OwnUserProfileProps) {
  return (
    <Container className="user-container">
      <Row className="justify-content-center">
        <Col xs={12}>
          <h1 className="user-header">Welcome back {username}</h1>
        </Col>
      </Row>
      <Row className="justify-content-center mt-3">
        <Col xs={12} md={5} lg={4}>
          <Accordion className="accordion-user">
            <Accordion.Item eventKey="0">
              <Accordion.Header>Your follows</Accordion.Header>
              <Accordion.Body>
                <FollowsList />
              </Accordion.Body>
            </Accordion.Item>
          </Accordion>
        </Col>
        <Col xs={12} md={5} lg={4}>
          <Accordion className="accordion-user">
            <Accordion.Item eventKey="1">
              <Accordion.Header>Your followers</Accordion.Header>
              <Accordion.Body>
                <FollowersList />
              </Accordion.Body>
            </Accordion.Item>
          </Accordion>
        </Col>
      </Row>
    </Container>
  );
}
