import { Button, Spinner } from 'react-bootstrap';
import { useFollow } from '../hooks/useFollow';

interface FollowButtonProps {
  userId: string;
}

export default function FollowButton({ userId }: FollowButtonProps) {
  const { error, isLoading, isFollowing, isUpdating, toggleFollow } = useFollow(userId, false);

  return (
    <>
      {error && <div className="text-danger">{error}</div>}
      <Button
        disabled={isLoading || isUpdating}
        onClick={toggleFollow}
      >
        {isLoading || isUpdating ? <Spinner size="sm" /> : isFollowing ? 'Unfollow' : 'Follow'}
      </Button>
    </>
  );
}