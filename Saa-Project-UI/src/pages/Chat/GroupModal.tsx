import styles from './Chat.module.css';
import { type UsersResponse } from "../../Types/UsersResponse";

interface GroupModalProps {
    contacts: UsersResponse[];
    groupName: string;
    setGroupName: (val: string) => void;
    groupDescription: string;
    setGroupDescription: (val: string) => void;
    selectedMemberIds: number[];
    toggleMember: (id: number) => void;
    onCreate: () => void;
    onClose: () => void;
}

const GroupModal = ({ contacts, groupName, setGroupName, selectedMemberIds, toggleMember, onCreate, onClose }: GroupModalProps) => {
    return (
        <div className={styles.modalOverlay}>
            <div className={styles.modalContent}>
                <h3>Create Group Chat</h3><br/>
                <div className={styles.formGroup}>
                    <label>Group Name</label>
                    <input className={styles.modalInput} value={groupName} onChange={(e) => setGroupName(e.target.value)} />
                </div>
                <div className={styles.memberSelection}>
                    <label>Select Members</label>
                    <div className={styles.memberList}>
                        {contacts.map(c => (
                            <div key={c.contactId} className={styles.memberItem}>
                                <span>{c.userName}</span>
                                <input type="checkbox" checked={selectedMemberIds.includes(c.contactId)} onChange={() => toggleMember(c.contactId)} />
                            </div>
                        ))}
                    </div>
                </div>
                <div className={styles.modalActions}>
                    <button onClick={onCreate} className={styles.saveBtn}>Create</button>
                    <button onClick={onClose} className={styles.cancelBtn}>Cancel</button>
                </div>
            </div>
        </div>
    );
};

export default GroupModal;