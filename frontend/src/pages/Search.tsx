import { useState } from "react";
import { useUserSearch } from "../hooks/useUserSearch";
import { ListGroup, Form, Spinner } from "react-bootstrap";
import { useNavigate } from "react-router-dom";

export default function SearchPage() {
  const [query, setQuery] = useState("");
  const { results, loading, error, search } = useUserSearch();
  const navigate = useNavigate();

  async function handleChange(e: React.ChangeEvent<HTMLInputElement>) {
    const value = e.target.value;
    setQuery(value);
    await search(value);
  }

  function goToProfile(id: string) {
    navigate(`/users/${id}`);
  }

  return (
    <div className="container py-4 text-light">
      <h3 className="mb-3 feed-title">Search users</h3>

      <Form.Control
        type="text"
        placeholder="Search by username..."
        value={query}
        onChange={handleChange}
        className="bg-dark text-light"
      />

      {loading && (
        <div className="mt-3">
          <Spinner animation="border" size="sm" /> Searching...
        </div>
      )}

      {error && <div className="text-danger mt-3">{error}</div>}

      <ListGroup className="mt-3">
        {results.map((u) => (
          <ListGroup.Item
            key={u.id}
            action
            onClick={() => goToProfile(u.id)}
            className="bg-transparent text-light border-secondary"
          >
            <i className="bi bi-person me-2"></i> {u.username}
          </ListGroup.Item>
        ))}

        {!loading && results.length === 0 && query.length >= 2 && (
          <div className="text-muted mt-3">No users found.</div>
        )}
      </ListGroup>
    </div>
  );
}
