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
      <Modal show={show} centered>
        <Modal.Header>
          <ButtonGroup>
            <Button
              variant={mode === "login" ? "primary" : "outline-primary"}
              onClick={() => handleSwitch("login")}
            >
              Login
            </Button>
            <Button
              variant={mode === "register" ? "primary" : "outline-primary"}
              onClick={() => handleSwitch("register")}
            >
              Register
            </Button>
          </ButtonGroup>
        </Modal.Header>
        <Modal.Body>
          {mode === "login" ? (
            <LoginForm onSuccess={handleLoginSuccess} />
          ) : (
            <RegisterForm
              onSuccess={handleLoginSuccess}
              onSwitchToLogin={handleRegistrationSuccess}
            />
          )}
        </Modal.Body>
      </Modal>
    </>
  );
}
