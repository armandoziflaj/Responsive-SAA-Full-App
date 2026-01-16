import React from 'react';
import { Link } from 'react-router-dom';

import './Navbar.css';

const Navbar: React.FC = () => {
    return (
        <nav className="navbar">
            <Link to="/" className="link">Home</Link>
            <Link to="/login" className="link" style={{ marginLeft: 'auto' }}>Login</Link>
            <Link to="/register" className="link">Register</Link>
        </nav>
    );
};

export default Navbar;