
import { Col, Container, Row } from "react-bootstrap"
import { Link } from "react-router-dom"
import comet from "../assets/comet.svg"
type Props = {}

export default function NotFound({ }: Props) {
  return (
    <Container fluid className="p-5 text-primary min-vh-100 max-vw-80"  >
      <Row className="justify-content-center align-items-center g-5">
        <Col xs={10} sm={6} md={6} lg={5}>
          <h1>Page Not found</h1>
          <p>Seems you've gotten lost among the stars...</p>
          <Link to={('/')}>Return to homebase</Link>
        </Col>
        <Col xs={10} sm={6} md={6} lg={5} className="px-2 py-5">
          <img src={comet} className="img-fluid" style={{ maxWidth: '70%' }} ></img>
        </Col>
      </Row>

    </Container >
  )
}