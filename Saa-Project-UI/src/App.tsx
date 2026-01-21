import {BrowserRouter as Router, Routes, Route} from 'react-router-dom';
import Navbar from './components/Navbar/Navbar.tsx';
import Login from "./pages/Login/Login.tsx";
import Footer from "./components/Footer/Footer.tsx";
import Register from "./pages/Register/Register.tsx";
import Posts from "./pages/Posts/PostPage.tsx";
import './App.css';

function App() {
    return (
        <Router>
            <div className="appContainer">
            <Navbar />
                <main>
                    <Routes>
                        <Route path="/login" element={<Login />} />
                        <Route path="/register" element={<Register />} />
                        <Route path="/posts" element={<Posts />} />
                    </Routes>
                </main>
                <Footer />
            </div>

        </Router>
    );
}

export default App
