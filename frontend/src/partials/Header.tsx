import { Container, Navbar, Nav } from "react-bootstrap";
import { Link } from "react-router-dom";

interface HeaderProps {
  isLoggedIn: boolean;
  username: string | null;
  onCreatePost: () => void;
  onSendMessage: () => void;
  onLogout: () => void;
  userId: string | null;
}

export default function Header({
  isLoggedIn,
  username,
  onCreatePost,
  onSendMessage,
  onLogout,
  userId,
}: HeaderProps) {
  return (
    <header className="app-header sticky-top">
      <Navbar expand="md" className="app-header-bar">
        <Container
          fluid
          className="d-flex justify-content-between align-items-center"
        >
          <Navbar.Brand
            as={Link}
            to="/"
            className="text-warning app-header-brand"
          >
            MySpace
          </Navbar.Brand>

          {isLoggedIn && (
            <Nav className="app-header-nav">
              <Nav.Link as={Link} to="/search" className="app-header-link">
                <i className="bi bi-search"></i>
              </Nav.Link>

              <Nav.Link className="app-header-link" onClick={onCreatePost}>
                <i className="bi bi-pencil" />
                <span className="d-none d-md-inline ms-1">Post</span>
              </Nav.Link>

              <Nav.Link className="app-header-link" onClick={onSendMessage}>
                <i className="bi bi-chat" />
                <span className="d-none d-md-inline ms-1">Message</span>
              </Nav.Link>

              <Nav.Link
                as={Link}
                to={userId ? `/users/${userId}/timeline` : "#"}
                className="app-header-link"
              >
                <i className="bi bi-person" />
                <span className="app-header-username text-light d-none d-md-inline ms-1">
                  {username}
                </span>
              </Nav.Link>

              <Nav.Link className="app-header-link" onClick={onLogout}>
                <i className="bi bi-door-open" />
                <span className="d-none d-md-inline ms-1">Logout</span>
              </Nav.Link>
            </Nav>
          )}
        </Container>
      </Navbar>
    </header>
  );
}
