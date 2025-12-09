import { Accordion, Col, Container, Row } from "react-bootstrap";
import FollowsList from "./FollowsList";

interface OwnUserProfileProps {
  username: string | null
}


export default function OwnUserProfile({ username }: OwnUserProfileProps) {

  return (
    <Container className="user-container">
      <Row className="justify-content-center">
        <Col xs={12} md={8} lg={6}>
          <h1 className="user-header">Welcome back {username}</h1>
          <Accordion className="accordion-user">
            <Accordion.Item eventKey="0">
              <Accordion.Header>
                Your follows
              </Accordion.Header>
              <Accordion.Body>

                <FollowsList />
              </Accordion.Body>
            </Accordion.Item>
          </Accordion>
        </Col>
      </Row>
    </Container >
  )
}
