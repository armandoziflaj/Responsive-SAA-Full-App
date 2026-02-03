import React, { useState, useEffect, useRef } from 'react';
import styles from './NotificationDropdown.module.css';
import api from '../../Services/AuthService';
import type { NotificationResponse } from "../../Types/NotificationResponse.ts";

const NotificationDropdown: React.FC = () => {
    const [notifications, setNotifications] = useState<NotificationResponse[]>([]);
    const isOpenState = useState(false);
    const [isOpen, setIsOpen] = isOpenState;
    const dropdownRef = useRef<HTMLDivElement>(null);
    const currentUserId = parseInt(localStorage.getItem('userId') || '0');

    // Derived state: unreadCount is simply the number of notifications in the list
    const unreadCount = notifications.length;

    useEffect(() => {
        const loadData = async () => {
            try {
                const res = await api.get('/api/Notifications');
                if (res.data.isSuccess) {
                    const data: NotificationResponse[] = res.data.data;
                    setNotifications(data);
                }
            } catch (err) {
                console.error("Initial fetch error:", err);
            }
        };

        loadData();

        const handleNewNotif = (event: Event) => {
            const customEvent = event as CustomEvent<NotificationResponse>;
            const newNotif = customEvent.detail;

            if (newNotif.relatedId === currentUserId) {
                return;
            }

            setNotifications(prev => {
                if (prev.some(n => n.id === newNotif.id)) return prev;
                return [newNotif, ...prev];
            });
        };

        window.addEventListener("app-notification", handleNewNotif);

        const handleClickOutside = (e: MouseEvent) => {
            if (dropdownRef.current && !dropdownRef.current.contains(e.target as Node)) {
                setIsOpen(false);
            }
        };

        document.addEventListener('mousedown', handleClickOutside);
        return () => {
            window.removeEventListener("app-notification", handleNewNotif);
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, []);

    const markAsRead = async (id: number) => {
        try {
            await api.patch(`/api/Notifications/${id}/read`);
            setNotifications(prev => prev.filter(n => n.id !== id));
        } catch (err) {
            console.error("Mark as read error:", err);
        }
    };
    const markAllAsRead = async () => {
        if (notifications.length === 0) return;

        try {
            const res = await api.patch('/api/Notifications/read-all');
            if (res.data.isSuccess) {
                setNotifications([]);
            }
        } catch (err) {
            console.error("Clear all error:", err);
        }
    };

    return (
        <div className={styles.wrapper} ref={dropdownRef}>
            <div className={styles.bellWrapper} onClick={() => setIsOpen(!isOpen)}>
                <i className={`fa fa-bell ${unreadCount > 0 ? styles.ring : ''}`}></i>
                {unreadCount > 0 && <span className={styles.badge}>{unreadCount}</span>}
            </div>

            {isOpen && (
                <div className={styles.dropdown}>
                    <div className={styles.header}>
                        <span>Notifications</span>
                        {notifications.length > 0 && (
                            <button
                                className={styles.clearAllBtn}
                                onClick={markAllAsRead}
                            >
                                Clear All
                            </button>
                        )}
                    </div>
                    <div className={styles.list}>
                        {notifications.length > 0 ? (
                            notifications.map(n => (
                                <div key={n.id} className={styles.item} onClick={() => markAsRead(n.id)}>
                                    <p>{n.message}</p>
                                    <small>
                                        {new Date(n.createdOn).toLocaleTimeString('el-GR', {
                                            hour: '2-digit',
                                            minute: '2-digit'
                                        })}
                                    </small>
                                </div>
                            ))
                        ) : (
                            <div className={styles.empty}>No new notifications</div>
                        )}
                    </div>
                </div>
            )}
        </div>
    );
};

export default NotificationDropdown;