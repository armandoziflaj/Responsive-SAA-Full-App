import {BrowserRouter as Router, Routes, Route} from 'react-router-dom';
import Navbar from './components/Navbar/Navbar.tsx';
import Login from "./pages/Login/Login.tsx";
import Footer from "./components/Footer/Footer.tsx";

// import Register from "./pages/Register.tsx";

function App() {
    return (
        <Router>
            <Navbar />
            <Routes>
                <Route path="/login" element={<Login />} />
                {/*<Route path="/register" element={<Register />} />*/}
            </Routes>
            <Footer />
        </Router>
    );
}

export default App
