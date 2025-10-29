// External imports
import React from "react";
import { Routes, Route, useNavigate, useLocation } from "react-router-dom";

// Internal imports
import Home from "./pages/Home";
import Dashboard from "./pages/Dashboard";
import "./styles/app.css";

function App() {
  const location = useLocation();

  return (
    <Routes location={location} key={location.pathname}>
      <Route path="/" element={<Home />} />
      <Route path="/dash" element={<Dashboard />} />
    </Routes>
  );
}

export default App;
