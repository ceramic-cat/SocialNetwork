import { useState } from "react";
import { Container, Navbar, Nav, Offcanvas } from "react-bootstrap";
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
  const [showOffcanvas, setShowOffcanvas] = useState(false);

  const handleClose = () => setShowOffcanvas(false);
  const handleShow = () => setShowOffcanvas(true);

  return (
    <header className="app-header sticky-top">
      <Navbar data-bs-theme="dark" className="app-header-bar">
        <Container fluid>
          <Navbar.Brand
            as={Link}
            to="/"
            className="text-primary app-header-brand audiowide fs-1 mx-4"
          >
            MySpace
          </Navbar.Brand>

          {isLoggedIn && (
            <>
              {/* Desktop Nav - hidden below md */}
              <Nav className="app-header-nav d-none d-md-flex ms-auto">
                <Nav.Link as={Link} to="/search" className="app-header-link">
                  <i className="bi bi-search"></i>
                  <span className="d-none d-md-inline ms-1">Search</span>
                </Nav.Link>

                <Nav.Link className="app-header-link" onClick={onCreatePost}>
                  <i className="bi bi-pencil" />
                  <span className="d-none d-md-inline ms-1">Post</span>
                </Nav.Link>

                <Nav.Link className="app-header-link" onClick={onSendMessage}>
                  <i className="bi bi-envelope" />
                  <span className="d-none d-md-inline ms-1">Message</span>
                </Nav.Link>

                <Nav.Link
                  as={Link}
                  to={userId ? `/users/${userId}` : "#"}
                  className="app-header-link"
                >
                  <i className="bi bi-person" />
                  <span className="app-header-username d-none d-md-inline ms-1">
                    {username}
                  </span>
                </Nav.Link>

                <Nav.Link className="app-header-link" onClick={onLogout}>
                  <i className="bi bi-door-open" />
                  <span className="d-none d-md-inline ms-1">Logout</span>
                </Nav.Link>
              </Nav>

              {/* Mobile hamburger - visible below md */}
              <button
                type="button"
                className="app-header-toggle d-flex d-md-none"
                aria-label="Toggle navigation"
                onClick={handleShow}
              >
                <i className="bi bi-list"></i>
              </button>

              {/* Mobile Offcanvas */}
              <Offcanvas
                show={showOffcanvas}
                onHide={handleClose}
                placement="end"
                data-bs-theme="dark"
                className="app-header-offcanvas"
              >
                <Offcanvas.Header closeButton>
                  <Offcanvas.Title className="audiowide fs-2">Menu</Offcanvas.Title>
                </Offcanvas.Header>
                <Offcanvas.Body>
                  <Nav className="flex-column gap-1">
                    <Nav.Link as={Link} to="/search" className="app-header-link" onClick={handleClose}>
                      <i className="bi bi-search me-2"></i>
                      <span>Search</span>
                    </Nav.Link>

                    <Nav.Link className="app-header-link" onClick={() => { onCreatePost(); handleClose(); }}>
                      <i className="bi bi-pencil me-2" />
                      <span>Post</span>
                    </Nav.Link>

                    <Nav.Link className="app-header-link" onClick={() => { onSendMessage(); handleClose(); }}>
                      <i className="bi bi-envelope me-2" />
                      <span>Message</span>
                    </Nav.Link>

                    <Nav.Link
                      as={Link}
                      to={userId ? `/users/${userId}` : "#"}
                      className="app-header-link"
                      onClick={handleClose}
                    >
                      <i className="bi bi-person me-2" />
                      <span className="app-header-username">{username}</span>
                    </Nav.Link>

                    <Nav.Link className="app-header-link" onClick={() => { onLogout(); handleClose(); }}>
                      <i className="bi bi-door-open me-2" />
                      <span>Logout</span>
                    </Nav.Link>
                  </Nav>
                </Offcanvas.Body>
              </Offcanvas>
            </>
          )}
        </Container>
      </Navbar>
    </header>
  );
}