import { useState } from "react";
import { Button, Form, Row, Col, Spinner, Alert } from "react-bootstrap";
import { useCreatePost } from "../hooks/useCreatePost";
import type { CreatePostRequest } from "../types/post";

const MAX_CONTENT_LENGTH = 280;

export interface CreatePostFormProps {
  senderId: string;
  senderName: string;
  onSuccess?: () => void;
}

export default function CreatePostForm({
  senderId,
  senderName,
  onSuccess,
}: CreatePostFormProps) {
  const [content, setContent] = useState("");
  const [validationError, setValidationError] = useState<string | null>(null);

  const {
    createPost,
    loading,
    error: apiError,
    success,
    resetStatus,
  } = useCreatePost();

  function validate(): string | null {
    if (!content.trim()) return "Content cannot be empty.";
    if (content.length > MAX_CONTENT_LENGTH) {
      return `Content cannot exceed ${MAX_CONTENT_LENGTH} characters.`;
    }
    return null;
  }

  const isFormInvalid = validate() !== null;

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();

    resetStatus();
    const validationMsg = validate();
    setValidationError(validationMsg);

    if (validationMsg) {
      return;
    }

    const payload: CreatePostRequest = {
      senderId,
      receiverId: senderId, // change here if posting on another's timeline
      content,
    };

    const ok = await createPost(payload);

    if (ok) {
      setContent("");

      if (onSuccess) {
        onSuccess();
      }
    }
  }

  function handleContentChange(event: React.ChangeEvent<HTMLTextAreaElement>) {
    setContent(event.target.value);
    if (validationError) {
      setValidationError(null);
    }
  }

  return (
    <Form onSubmit={handleSubmit}>
      <Row className="mb-3">
        <Col>
          <h5 className="mb-2">{senderName}</h5>
        </Col>
      </Row>

      {apiError && (
        <Alert variant="danger" className="py-2">
          {apiError}
        </Alert>
      )}
      {validationError && !apiError && (
        <Alert variant="warning" className="py-2">
          {validationError}
        </Alert>
      )}

      <Row className="mb-3">
        <Col>
          <Form.Group>
            <Form.Control
              as="textarea"
              rows={4}
              value={content}
              onChange={handleContentChange}
              className="rounded-3"
              isInvalid={
                !!validationError &&
                (validationError.toLowerCase().includes("content") ||
                  validationError.toLowerCase().includes("exceed"))
              }
            />
            <Form.Control.Feedback type="invalid">
              {validationError}
            </Form.Control.Feedback>
          </Form.Group>
        </Col>
      </Row>

      <Row>
        <Col>
          <div className="d-grid">
            <Button type="submit" disabled={loading || isFormInvalid}>
              {loading ? (
                <>
                  <Spinner size="sm" animation="border" className="me-2" />
                  Posting...
                </>
              ) : (
                "Post"
              )}
            </Button>
          </div>
        </Col>
      </Row>
    </Form>
  );
}
