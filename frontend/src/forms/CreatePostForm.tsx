import { useState } from "react";
import { Button, Form, Row, Col, Spinner, Alert } from "react-bootstrap";
import { useCreatePost } from "../hooks/useCreatePost";
import type { CreatePostRequest } from "../types/post";
import FeedbackAlert from "../alerts/FeedbackAlert";

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

  function validate(value: string): string | null {
    if (!value.trim()) return "Content cannot be empty.";
    if (value.length > MAX_CONTENT_LENGTH) {
      return `Content cannot exceed ${MAX_CONTENT_LENGTH} characters.`;
    }
    return null;
  }

  const isFormInvalid = validate(content) !== null;

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();

    resetStatus();
    const validationMsg = validate(content);
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
        setTimeout(() => {
          onSuccess();
        }, 1500);
      }
    }
  }

  function handleContentChange(event: React.ChangeEvent<HTMLTextAreaElement>) {
    const newValue = event.target.value;
    setContent(newValue);
    setValidationError(validate(newValue));
  }

  return (
    <Form onSubmit={handleSubmit}>
      <Row className="mb-3">
        <Col>
          <h5 className="mb-2 sender-name">{senderName}</h5>
        </Col>
      </Row>

      <FeedbackAlert
        error={validationError || apiError}
        className={
          validationError || apiError ? "alert-form-danger" : undefined
        }
        success={success ? "Post created successfully!" : undefined}
      />

      <Row className="mb-3">
        <Col>
          <Form.Group>
            <Form.Control
              as="textarea"
              rows={4}
              value={content}
              onChange={handleContentChange}
              className="rounded-3 custom-scroll"
              isInvalid={!!validationError}
            />
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
