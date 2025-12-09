
import { useGetFollowsInfo } from '../hooks/useGetFollowsInfo';
import { Alert, ListGroup, Spinner } from 'react-bootstrap';
import { useNavigate } from "react-router-dom";

type Props = {}

export default function FollowsList({ }: Props) {
  const { isLoading, error, follows } = useGetFollowsInfo();
  const navigate = useNavigate();
  const followsEmpty = follows.length === 0 ? true : false;

  return (
    <div className='container py-4 text-light'>
      <h4 className='text-white pt-4'>All the accounts you follow</h4>
      {isLoading ?
        <Spinner className='align-center' /> :
        error ?
          <Alert>{error}</Alert> :
          followsEmpty ?
            <p>You follow no other users.</p>
            :
            <ListGroup>
              {follows.map((follows) => (
                <ListGroup.Item
                  key={follows.id}
                  action
                  onClick={() => navigate(`/users/${follows.id}`)}
                  className='bg-transparent text-light border-secondary'>
                  {follows.username}

                </ListGroup.Item>
              ))}
            </ListGroup>
      }
    </div>
  )
}