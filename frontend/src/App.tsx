import { Route, Routes } from "react-router-dom";
import Home from "./pages/Home";
import Timeline from "./pages/Timeline";

function App() {
  return (
    <div className="mt-3 mb-3">
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/timeline" element={<Timeline />} />
      </Routes>
    </div>
  );
}

export default App;
