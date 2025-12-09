import Timeline from '../timeline/Timeline'
import { useParams } from 'react-router-dom';
import useCurrentUser from '../hooks/useCurrentUser';
import OwnUserProfile from '../UserProfile/OwnUserProfile';
import OtherUserProfile from '../UserProfile/OtherUserProfile';


export default function User() {
  const { id: userId } = useParams<{ id: string }>();
  const { userId: currentUserId, username: currentUsername } = useCurrentUser();

  const isOwnPage = userId === currentUserId;

  if (!userId) return <div>User not found</div>;

  return (
    <>
      {isOwnPage ? (
        <OwnUserProfile username={currentUsername} />
      ) : (
        <OtherUserProfile userId={userId} />
      )}

      <Timeline />
    </>
  )
}