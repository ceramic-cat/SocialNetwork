import Timeline from '../timeline/Timeline'
import { useParams } from 'react-router-dom';
import useCurrentUser from '../hooks/useCurrentUser';
import FollowButton from '../partials/FollowButton';


export default function User() {
  const { id: userId } = useParams<{ id: string }>();
  const { userId: currentUserId, username } = useCurrentUser();

  const isOwnPage = userId === currentUserId;

  if (!userId) return <div>User not found</div>;

  return (
    <>
      {isOwnPage ? (
        <div>
          <h1 className="text-white">Welcome back {username}</h1>
        </div>
      ) : (
        <div>
          <h1 className="text-primary">Welcome to {userId}</h1>
          <FollowButton userId={userId} />
        </div>
      )}

      <Timeline />
    </>
  )
}