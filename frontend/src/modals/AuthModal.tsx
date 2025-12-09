import { useState } from "react";
import { Button, Modal, ButtonGroup } from "react-bootstrap";
import LoginForm from "../forms/LoginForm";
import RegisterForm from "../forms/RegisterForm";

interface AuthModalProps {
  onLoginSuccess?: () => void;
  showInitially?: boolean;
}

export default function AuthModal({
  onLoginSuccess,
  showInitially = false,
}: AuthModalProps) {
  const [show, setShow] = useState(showInitially);
  const [mode, setMode] = useState<"login" | "register">("login");
  const [registrationSuccess, setRegistrationSuccess] = useState(false);

  function handleClose() {
    setShow(false);
  }

  function handleSwitch(newMode: "login" | "register") {
    setMode(newMode);
    setRegistrationSuccess(false);
  }

  function handleLoginSuccess() {
    handleClose();
    onLoginSuccess?.();
  }

  function handleRegistrationSuccess() {
    setRegistrationSuccess(true);
    setMode("login");
  }

  return (
    <>
      <Modal show={show} centered className="auth-modal">
        <div className="auth-modal-content">
          <div className="background-animation"></div>
          <div className="floating-elements">
            <div className="element element-1"></div>
            <div className="element element-2"></div>
            <div className="element element-3"></div>
          </div>

          <Modal.Header className="auth-modal-header">
            <div className="modal-title">
              <h3 className="title audiowide">MySpace</h3>
              <p className="subtitle">Creativity and social connection</p>
            </div>
            <ButtonGroup className="auth-tab-group">
              <Button
                className={`auth-tab ${mode === "login" ? "active" : ""}`}
                onClick={() => handleSwitch("login")}
              >
                Login
              </Button>
              <Button
                className={`auth-tab ${mode === "register" ? "active" : ""}`}
                onClick={() => handleSwitch("register")}
              >
                Register
              </Button>
            </ButtonGroup>
          </Modal.Header>

          <Modal.Body className="auth-modal-body">
            {mode === "login" ? (
              <LoginForm
                onSuccess={handleLoginSuccess}
                showRegistrationSuccess={registrationSuccess}
              />
            ) : (
              <RegisterForm
                onSuccess={handleLoginSuccess}
                onSwitchToLogin={handleRegistrationSuccess}
              />
            )}
          </Modal.Body>
        </div>
      </Modal>
    </>
  );
}
