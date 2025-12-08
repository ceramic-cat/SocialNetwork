import Timeline from '../timeline/Timeline'
import { useParams } from 'react-router-dom';
import useCurrentUser from '../hooks/useCurrentUser';
import { useIsFollowing } from '../hooks/useIsFollowing';
import { Button, Spinner } from 'react-bootstrap';


export default function User() {
  const { id: userId } = useParams<{ id: string }>();
  const { userId: currentUserId, username } = useCurrentUser();

  const isOwnPage = userId === currentUserId;
  const { error, isLoading, isFollowing } = useIsFollowing(userId, isOwnPage);




  return (
    <>

      {isOwnPage &&
        <div>
          <h1 className='text-white'>Welcome back {username}</h1>
        </div>
      }
      {!isOwnPage &&
        <div>
          <h1 className='text-primary'>Welcome to {userId}</h1>
          {error && <div className='text-primary'> {error}</div>}
          <Button
            disabled={isLoading}
          >{isLoading ? 'Loading...' : isFollowing ? 'Unfollow' : 'Follow'}</Button>
        </div>
      }

      <Timeline />
    </>
  )
}