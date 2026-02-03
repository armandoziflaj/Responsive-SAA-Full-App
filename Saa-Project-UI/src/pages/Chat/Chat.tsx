import { useEffect, useState } from 'react';
import api from '../../Services/AuthService';
import styles from './Chat.module.css';
import Sidebar from './Sidebar';
import ChatWindow from './ChatWindow';
import GroupModal from './GroupModal';
import { useSignalR } from '../../Context/SignalRContext.tsx';
import { type ActiveChat } from '../../Types/ChatTypes';
import { type BaseResponse } from "../../Types/ApiResponses";
import { type UsersResponse } from "../../Types/UsersResponse";
import { type MessagesResponse } from "../../Types/MessagesResponse";
import { type GroupsResponse } from "../../Types/GroupsResponse";

const Chat = () => {
    const { connection, isJoined } = useSignalR();

    const [contacts, setContacts] = useState<UsersResponse[]>([]);
    const [groups, setGroups] = useState<GroupsResponse[]>([]);
    const [messages, setMessages] = useState<MessagesResponse[]>([]);
    const [activeChat, setActiveChat] = useState<ActiveChat | null>(null);
    const [newMessage, setNewMessage] = useState("");

    const [isGroupModalOpen, setIsGroupModalOpen] = useState(false);
    const [groupName, setGroupName] = useState('');
    const [groupDescription, setGroupDescription] = useState('');
    const [selectedMemberIds, setSelectedMemberIds] = useState<number[]>([]);

    const currentUserId = parseInt(localStorage.getItem('userId') || '0');

    useEffect(() => {
        const handleIncomingMessage = (event: Event) => {
            const customEvent = event as CustomEvent<MessagesResponse>;
            const msg = customEvent.detail;

            console.log("📩 SignalR Event Received:", msg);

            setActiveChat(current => {
                if (!current) return current;

                const isGroupMatch = current.type === 'group' && msg.groupId === current.id;
                const isPrivateMatch = current.type === 'private' && (
                    (msg.senderId === current.id && msg.receiverId === currentUserId) ||
                    (msg.senderId === currentUserId && msg.receiverId === current.id)
                );

                if (isGroupMatch || isPrivateMatch || msg.senderId === currentUserId) {
                    setMessages(prev => {
                        if (prev.some(m => m.id === msg.id)) return prev;
                        return [...prev, msg];
                    });
                }
                return current;
            });
        };

        window.addEventListener("app-message", handleIncomingMessage);
        return () => window.removeEventListener("app-message", handleIncomingMessage);
    }, [currentUserId]);

    useEffect(() => {
        if (connection && isJoined && activeChat) {
            if (activeChat.type === 'group') {
                const roomId = `group_${activeChat.id}`;
                connection.invoke("JoinChat", roomId).catch(err => console.error(err));
            }
        }
    }, [activeChat, connection, isJoined]);

    useEffect(() => {
        const fetchSidebar = async () => {
            try {
                const [c, g] = await Promise.all([
                    api.get<BaseResponse<UsersResponse[]>>('/api/Contacts'),
                    api.get<BaseResponse<GroupsResponse[]>>('/api/Group')
                ]);
                if (c.data.isSuccess) setContacts(c.data.data);
                if (g.data.isSuccess) setGroups(g.data.data);
            } catch (err) { console.error("Sidebar error", err); }
        };
        fetchSidebar();
    }, []);


    useEffect(() => {
        if (!activeChat) return;
        api.get<BaseResponse<MessagesResponse[]>>(`/api/Messages/${activeChat.type}/${activeChat.id}`)
            .then(res => {
                if (res.data.isSuccess) setMessages(res.data.data);
            });
    }, [activeChat]);

    const handleSendMessage = async () => {
        if (!newMessage.trim() || !activeChat) return;
        const payload = activeChat.type === 'private'
            ? { receiverId: activeChat.id, content: newMessage }
            : { groupId: activeChat.id, content: newMessage };

        try {
            await api.post(`/api/Messages/${activeChat.type}`, payload);
            setNewMessage("");
        } catch (err) { console.error("Send error", err); }
    };

    const handleCreateGroup = async () => {
        const res = await api.post('/api/Group', {
            name: groupName,
            description: groupDescription,
            UserIds: selectedMemberIds
        });
        if (res.data.isSuccess) {
            setIsGroupModalOpen(false);
            setGroupName('');
            setSelectedMemberIds([]);
            const groupRes = await api.get<BaseResponse<GroupsResponse[]>>('/api/Group');
            setGroups(groupRes.data.data);
        }
    };

    return (
        <div className={styles.chatContainer}>
            <Sidebar
                contacts={contacts} groups={groups}
                activeChat={activeChat} setActiveChat={setActiveChat}
                openModal={() => setIsGroupModalOpen(true)}
            />

            {activeChat ? (
                <ChatWindow
                    activeChat={activeChat} messages={messages}
                    currentUserId={currentUserId} newMessage={newMessage}
                    setNewMessage={setNewMessage} onSendMessage={handleSendMessage}
                />
            ) : (
                <div className={styles.emptyState}><p>Επιλέξτε μια συνομιλία για να ξεκινήσετε</p></div>
            )}

            {isGroupModalOpen && (
                <GroupModal
                    contacts={contacts} groupName={groupName} setGroupName={setGroupName}
                    groupDescription={groupDescription} setGroupDescription={setGroupDescription}
                    selectedMemberIds={selectedMemberIds} onCreate={handleCreateGroup}
                    onClose={() => setIsGroupModalOpen(false)}
                    toggleMember={(id) => setSelectedMemberIds(prev => prev.includes(id) ? prev.filter(x => x !== id) : [...prev, id])}
                />
            )}
        </div>
    );
};

export default Chat;