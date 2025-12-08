import { Alert, Spinner, Tab, Tabs } from "react-bootstrap";
import { useGetFollows } from "../hooks/useGetFollows";

interface OwnUserProfileProps {
  username: string | null
}


export default function OwnUserProfile({ username }: OwnUserProfileProps) {

  const { isLoading, error, follows } = useGetFollows();


  return (
    <div className='user-container'>
      <h1 className="user-header">Welcome back {username}</h1>
      <Tabs defaultActiveKey="general">
        <Tab eventKey="general" title="General">
          <p className='text-white'>Information maybe?</p>
        </Tab>
        <Tab eventKey="follows" title="Follows">
          <h4 className='text-white pt-4'>All the accounts you follow</h4>
          {isLoading ?
            <Spinner /> :
            error ?
              <Alert>{error}</Alert> :
              <p className="text-primary">Everything you follow</p>}


        </Tab>
      </Tabs>

    </div>
  )
}
