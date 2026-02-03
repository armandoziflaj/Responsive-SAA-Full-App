import {BrowserRouter as Router, Routes, Route} from 'react-router-dom';
import Navbar from './components/Navbar/Navbar.tsx';
import Login from "./pages/Login/Login.tsx";
import Footer from "./components/Footer/Footer.tsx";
import Register from "./pages/Register/Register.tsx";
import Posts from "./pages/Posts/PostPage.tsx";
import Chat from "./pages/Chat/Chat.tsx";
import './App.css';
import { SignalRProvider } from './Context/SignalRProvider';
import LoginSuccess from "./pages/Login/LoginSuccess.tsx";

function App() {
    return (
        <SignalRProvider>
        <Router>
            <div className="appContainer">
            <Navbar />
                <main>
                    <Routes>
                        <Route path="/login-success" element={<LoginSuccess />} />
                        <Route path="/login" element={<Login />} />
                        <Route path="/register" element={<Register />} />
                        <Route path="/posts" element={<Posts />} />
                        <Route path="/Chats" element={<Chat />} />
                    </Routes>
                </main>
                <Footer />
            </div>

        </Router>
        </SignalRProvider>
    );
}

export default App
