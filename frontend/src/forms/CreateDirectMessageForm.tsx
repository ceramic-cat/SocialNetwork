import { useState, useEffect, useRef } from "react";
import { Button, Form, Row, Col, Spinner, ListGroup } from "react-bootstrap";
import { useSendDirectMessage } from "../hooks/useSendDirectMessage";
import type { CreateDirectMessageRequest } from "../types/directMessage";
import FeedbackAlert from "../alerts/FeedbackAlert";
import { useUserSearch, type UserSearchResult } from "../hooks/useUserSearch";
import useGetUsername from "../hooks/useGetUsername";
import { DirectMessageErrors } from "../constants/directMessageErrors";

const MAX_CONTENT_LENGTH = 280;

export interface CreateDirectMessageFormProps {
  onSuccess?: () => void;
  receiverId?: string;
}

export default function CreateDirectMessageForm({
  onSuccess,
  receiverId: initialReceiverId,
}: CreateDirectMessageFormProps) {
  const [receiverId, setReceiverId] = useState(initialReceiverId || "");
  const [content, setContent] = useState("");
  const [validationError, setValidationError] = useState<string | null>(null);
  const [searchQuery, setSearchQuery] = useState("");
  const [selectedUser, setSelectedUser] = useState<UserSearchResult | null>(null);
  const [showSearchResults, setShowSearchResults] = useState(false);
  const [isSelectingUser, setIsSelectingUser] = useState(false);
  const searchRef = useRef<HTMLDivElement>(null);
  
  const { results: searchResults, loading: searchLoading, search: searchUsers } = useUserSearch();
  const { username: receiverUsername } = useGetUsername(initialReceiverId || undefined);

  // Update receiverId when initialReceiverId changes
  useEffect(() => {
    if (initialReceiverId) {
      setReceiverId(initialReceiverId);
    }
  }, [initialReceiverId]);

  // Handle click outside to close search results
  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (searchRef.current && !searchRef.current.contains(event.target as Node)) {
        setShowSearchResults(false);
      }
    }

    document.addEventListener("mousedown", handleClickOutside);
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, []);

  // Search users when query changes (with debounce)
  useEffect(() => {
    // Don't search if user just selected a user
    if (isSelectingUser) {
      setIsSelectingUser(false);
      return;
    }

    const trimmed = searchQuery.trim();
    
    if (trimmed.length < 2) {
      setShowSearchResults(false);
      return;
    }

    // Debounce search to avoid too many API calls
    const timeoutId = setTimeout(() => {
      searchUsers(trimmed);
      setShowSearchResults(true);
    }, 300); // Wait 300ms after user stops typing

    return () => {
      clearTimeout(timeoutId);
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [searchQuery]); // Only depend on searchQuery, not searchUsers

  function handleUserSelect(user: UserSearchResult, event?: React.MouseEvent) {
    if (event) {
      event.preventDefault();
      event.stopPropagation();
    }
    
    // Set flag to prevent search from triggering
    setIsSelectingUser(true);
    setShowSearchResults(false);
    
    // Update state
    setReceiverId(user.id);
    setSelectedUser(user);
    setSearchQuery(user.username);
    
    // Force close by blurring the input to prevent focus events
    setTimeout(() => {
      if (searchRef.current) {
        const input = searchRef.current.querySelector('input') as HTMLInputElement;
        if (input) {
          input.blur();
        }
      }
    }, 0);
  }

  function handleSearchChange(event: React.ChangeEvent<HTMLInputElement>) {
    const value = event.target.value;
    setSearchQuery(value);
    if (!value) {
      setReceiverId("");
      setSelectedUser(null);
      setShowSearchResults(false);
      // Clear validation error when clearing search
      setValidationError(null);
    } else {
      // Validate when user is typing (if content exists)
      if (content.trim()) {
        const validationMsg = validate("", content);
        setValidationError(validationMsg);
      }
    }
  }

  const {
    sendDirectMessage,
    loading,
    error: apiError,
    success,
    resetStatus,
  } = useSendDirectMessage();

  function validate(
    receiverIdValue?: string,
    contentValue?: string
  ): string | null {
    const receiverIdToCheck = receiverIdValue ?? receiverId;
    const contentToCheck = contentValue ?? content;

    if (!receiverIdToCheck.trim()) return DirectMessageErrors.ReceiverEmpty;
    if (!contentToCheck.trim()) return DirectMessageErrors.ContentEmpty;
    if (contentToCheck.length > MAX_CONTENT_LENGTH) {
      return DirectMessageErrors.ContentTooLong;
    }
    return null;
  }

  const isFormInvalid = validate() !== null;

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();

    // Always validate before submitting
    const validationMsg = validate();
    if (validationMsg) {
      resetStatus(); // Clear API errors, but keep validation error
      setValidationError(validationMsg);
      return;
    }
    
    // Clear validation error if validation passes
    setValidationError(null);
    resetStatus(); // Clear any previous API errors

    const payload: CreateDirectMessageRequest = {
      receiverId: receiverId.trim(),
      content: content.trim(),
    };

    const ok = await sendDirectMessage(payload);

    if (ok) {
      setReceiverId("");
      setContent("");
      setSearchQuery("");
      setSelectedUser(null);
      setShowSearchResults(false);

      if (onSuccess) {
        setTimeout(() => {
          onSuccess();
        }, 1500);
      }
    }
  }

  // Removed handleReceiverIdChange as we now use search-based selection

  function handleContentChange(event: React.ChangeEvent<HTMLTextAreaElement>) {
    const newValue = event.target.value;
    setContent(newValue);
    // Real-time validation - validate with current receiver ID and new content
    const validationMsg = validate(receiverId, newValue);
    setValidationError(validationMsg);
  }

  return (
    <Form onSubmit={handleSubmit}>
      <FeedbackAlert
        error={validationError || apiError}
        className={
          validationError || apiError ? "alert-form-danger" : undefined
        }
        success={success ? "Direct message sent successfully!" : undefined}
      />

      <Row className="mb-3">
        <Col>
          <Form.Group>
            <Form.Label>Send to</Form.Label>
            <div ref={searchRef} style={{ position: "relative" }}>
              <Form.Control
                type="text"
                value={initialReceiverId ? (receiverUsername || "Loading...") : searchQuery}
                onChange={handleSearchChange}
                placeholder="Search by username..."
                className="rounded-3"
                disabled={!!initialReceiverId}
                isInvalid={
                  !!validationError &&
                  validationError.toLowerCase().includes("receiver")
                }
                onFocus={() => {
                  // Don't show results if user just selected one, or if there's a selected user
                  if (isSelectingUser || selectedUser) {
                    return;
                  }
                  if (searchQuery.trim().length >= 2 && !initialReceiverId && searchResults.length > 0) {
                    setShowSearchResults(true);
                  }
                }}
              />
              {showSearchResults && !initialReceiverId && searchQuery.trim().length >= 2 && (
                <div
                  style={{
                    position: "absolute",
                    top: "100%",
                    left: 0,
                    right: 0,
                    zIndex: 1000,
                    backgroundColor: "#2a2a3d",
                    border: "1px solid #8a4dc7",
                    borderRadius: "0.375rem",
                    marginTop: "0.25rem",
                    maxHeight: "200px",
                    overflowY: "auto",
                  }}
                  className="custom-scroll"
                  onMouseDown={(e) => e.preventDefault()}
                >
                  {searchLoading ? (
                    <div className="p-3 text-center">
                      <Spinner size="sm" animation="border" /> Searching...
                    </div>
                  ) : searchResults.length > 0 ? (
                    <ListGroup variant="flush">
                      {searchResults.map((user) => (
                        <ListGroup.Item
                          key={user.id}
                          action
                          onClick={(e) => handleUserSelect(user, e)}
                          onMouseDown={(e) => e.preventDefault()}
                          style={{
                            backgroundColor: "transparent",
                            color: "#e0d4f7",
                            cursor: "pointer",
                            border: "none",
                          }}
                          onMouseEnter={(e) => {
                            e.currentTarget.style.backgroundColor = "#5b2ca7";
                          }}
                          onMouseLeave={(e) => {
                            e.currentTarget.style.backgroundColor = "transparent";
                          }}
                        >
                          <i className="bi bi-person me-2"></i> {user.username}
                        </ListGroup.Item>
                      ))}
                    </ListGroup>
                  ) : (
                    <div className="p-3 text-muted text-center">No users found</div>
                  )}
                </div>
              )}
              {selectedUser && !initialReceiverId && (
                <Form.Text className="text-muted">
                  Selected: {selectedUser.username}
                </Form.Text>
              )}
              {initialReceiverId && (
                <Form.Text className="text-muted">
                  Sending message to this user
                </Form.Text>
              )}
            </div>
            <Form.Control.Feedback type="invalid">
              {validationError}
            </Form.Control.Feedback>
          </Form.Group>
        </Col>
      </Row>

      <Row className="mb-3">
        <Col>
          <Form.Group>
            <Form.Label>Message</Form.Label>
            <Form.Control
              as="textarea"
              rows={4}
              value={content}
              onChange={handleContentChange}
              placeholder="Enter your message (max 280 characters)"
              className="rounded-3 custom-scroll"
              isInvalid={
                !!validationError &&
                (validationError.toLowerCase().includes("content") ||
                  validationError.toLowerCase().includes("exceed"))
              }
            />
            <Form.Text className="text-muted">
              {content.length}/{MAX_CONTENT_LENGTH} characters
            </Form.Text>
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
                  Sending...
                </>
              ) : (
                "Send Message"
              )}
            </Button>
          </div>
        </Col>
      </Row>
    </Form>
  );
}
