import { Route, Routes } from "react-router-dom";
import TheFeed from "./pages/TheFeed";

function App() {
  return (
    <div className="mt-3 mb-3">
      <Routes>
        <Route path="/" element={<TheFeed />} />
      </Routes>
    </div>
  );
}

export default App;
