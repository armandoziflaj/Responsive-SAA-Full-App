import styles from './Chat.module.css';
import { type UsersResponse } from "../../Types/UsersResponse.ts";
import { type GroupsResponse } from "../../Types/GroupsResponse.ts";
import { type ActiveChat } from "../../Types/ChatTypes.ts";

interface SidebarProps {
    contacts: UsersResponse[];
    groups: GroupsResponse[];
    activeChat: ActiveChat | null;
    setActiveChat: (chat: ActiveChat) => void;
    openModal: () => void;
}

const Sidebar = ({ contacts, groups, activeChat, setActiveChat, openModal }: SidebarProps) => {
    return (
        <div className={styles.sidebar}>
            <div className={styles.sidebarHeader}>
                <h2 className={styles.sidebarTitle}>Messages</h2>
                <button className={styles.addGroupBtn} onClick={openModal}>
                    <i className="fas fa-plus-circle"></i>
                </button>
            </div>
            <div className={styles.listScroll}>
                {groups.length > 0 && <div className={styles.sectionLabel}>Groups</div>}
                {groups.map(g => (
                    <div
                        key={`group-${g.id}`}
                        className={`${styles.contactItem} ${activeChat?.id === g.id && activeChat.type === 'group' ? styles.active : ''}`}
                        onClick={() => setActiveChat({ type: 'group', id: g.id, name: g.name })}
                    >
                        <div className={`${styles.avatar} ${styles.groupAvatar}`}><i className="fa fa-users"></i></div>
                        <span>{g.name}</span>
                    </div>
                ))}
                <div className={styles.sectionLabel}>Private</div>
                {contacts.map(c => (
                    <div
                        key={`user-${c.contactId}`}
                        className={`${styles.contactItem} ${activeChat?.id === c.contactId && activeChat.type === 'private' ? styles.active : ''}`}
                        onClick={() => setActiveChat({ type: 'private', id: c.contactId, name: c.userName })}
                    >
                        <div className={styles.avatar}>{c.userName[0].toUpperCase()}</div>
                        <span>{c.userName}</span>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default Sidebar;