import { Alert, Spinner } from "react-bootstrap";
import FollowButton from "../partials/FollowButton";
import useGetUsername from "../hooks/useGetUsername";


type OtherUserProfileProps = {
  userId: string
}

export default function OtherUserProfile({ userId }: OtherUserProfileProps) {

  const { error, isLoading, username } = useGetUsername(userId);

  return (
    <div>
      {isLoading ? <Spinner /> : error ? <Alert>{error}</Alert> :
        <div className='user-container'>
          <h1 className="user-header">Welcome to {username}</h1>
          <FollowButton userId={userId} />
        </div>
      }
    </div>
  )
}