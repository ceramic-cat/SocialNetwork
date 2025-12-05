import { Container, Navbar, Nav } from "react-bootstrap";
import { Link } from "react-router-dom";

interface HeaderProps {
  isLoggedIn: boolean;
  username: string | null;
  onCreatePost: () => void;
  onSendMessage: () => void;
  onLogout: () => void;
}

export default function Header({
  isLoggedIn,
  username,
  onCreatePost,
  onSendMessage,
  onLogout,
}: HeaderProps) {
  return (
    <header className="app-header sticky-top">
      <Navbar expand="md" className="app-header-bar">
        <Container
          fluid
          className="d-flex justify-content-between align-items-center"
        >
          <Navbar.Brand as={Link} to="/" className="app-header-brand">
            Social Network
          </Navbar.Brand>

          {isLoggedIn && (
            <Nav className="d-flex align-items-center gap-3">
              <Nav.Link
                className="app-header-link d-flex align-items-center gap-1"
                onClick={onCreatePost}
              >
                <i className="bi bi-pencil" />
                <span className="d-none d-md-inline">Post</span>
              </Nav.Link>

              <Nav.Link
                className="app-header-link d-flex align-items-center gap-1"
                onClick={onSendMessage}
              >
                <i className="bi bi-chat" />
                <span className="d-none d-md-inline">Message</span>
              </Nav.Link>

              <span className="app-header-username d-none d-md-inline">
                {username}
              </span>

              <Nav.Link
                className="app-header-link d-flex align-items-center gap-1"
                onClick={onLogout}
              >
                <i className="bi bi-door-open" />
                <span className="d-none d-md-inline">Logout</span>
              </Nav.Link>
            </Nav>
          )}
        </Container>
      </Navbar>
    </header>
  );
}
