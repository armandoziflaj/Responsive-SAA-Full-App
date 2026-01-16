import React from 'react';
import { Link } from 'react-router-dom';
import styles from './Footer.module.css';

const Footer: React.FC = () => {
    const currentYear = new Date().getFullYear();

    return (
        <footer className={styles.footer}>
            <div className={styles.links}>
                <Link to="/about">About Us</Link>
                <Link to="/privacy">Privacy Policy</Link>
                <Link to="/terms">Terms of Service</Link>
            </div>
            <div className={styles.copyright}>
                &copy; {currentYear} Saa-Project. All rights reserved.
            </div>
        </footer>
    );
};

export default Footer;