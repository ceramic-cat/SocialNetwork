import Timeline from '../timeline/Timeline'
import { useParams } from 'react-router-dom';
import useCurrentUser from '../hooks/useCurrentUser';
import FollowButton from '../partials/FollowButton';
import useGetUsername from '../hooks/useGetUsername';
import { Alert, Spinner } from 'react-bootstrap';


export default function User() {
  const { id: userId } = useParams<{ id: string }>();
  const { userId: currentUserId, username: currentUsername } = useCurrentUser();
  const { error, isLoading, username } = useGetUsername(userId);

  const isOwnPage = userId === currentUserId;

  if (!userId) return <div>User not found</div>;

  console.log(username);

  return (
    <>
      {isOwnPage ? (
        <div>
          <h1 className="user-header">Welcome back {currentUsername}</h1>
        </div>
      ) : (
        <div>
          {isLoading ? <Spinner /> : error ? <Alert>{error}</Alert> :
            <>
              <h1 className="user-header">Welcome to {username}</h1>
              <FollowButton userId={userId} />
            </>
          }
        </div>
      )}

      <Timeline />
    </>
  )
}