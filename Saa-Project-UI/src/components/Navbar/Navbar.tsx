import { Link } from "react-router-dom";
//import type { BaseResponse } from "../../Types/ApiResponses.ts";
//import api from "../../Services/AuthService.ts";
import React, { useEffect } from "react";
import styles from "./Navbar.module.css";
import NotificationDropdown from "../NotificationDropdown/NotificationDropdown.tsx";

const Navbar: React.FC = () => {
    // const [unreadCount] = useState(0);
    const currentUserId = parseInt(localStorage.getItem('userId') || '0');

    const isGuest = currentUserId === 0;

    useEffect(() => {
        // ... το fetchNotifications παραμένει ίδιο ...
    }, []);

    return (
        <nav className={styles.navbar}>
            <Link to="/Posts" className={styles.link}>Home</Link>

            <div className={styles.navRight}>
                {!isGuest && (
                    <>
                        {/*<div className={styles.notificationContainer}>*/}
                        {/*    <Link to="/notifications" className={styles.link}>*/}
                        {/*        <i className={`fa fa-bell ${styles.notificationIcon}`}></i>*/}
                        {/*        {unreadCount > 0 && (*/}
                        {/*            <span className={styles.badge}>{unreadCount}</span>*/}
                        {/*        )}*/}
                        {/*    </Link>*/}
                        {/*</div>*/}
                        <div className={styles.navRight}>
                            {currentUserId && <NotificationDropdown />} {/* Το component σου! */}
                        </div>
                        <Link to="/Chats" className={styles.notificationContainer}>
                            <i className={`fa-solid fa-comment ${styles.chatIcon}`}></i>
                        </Link>
                    </>
                )}

                {isGuest ? (
                    <>
                        <Link to="/login" className={`${styles.link} ${styles.loginLink}`}>Login</Link>
                        <Link to="/register" className={`${styles.link} ${styles.registerLink}`}>Register</Link>
                    </>
                ) : (
                    <button
                        className={styles.loginLink}
                        onClick={() => { localStorage.clear(); window.location.href = '/login'; }}
                    >
                        Logout
                    </button>
                )}
            </div>
        </nav>
    );
};

export default Navbar;