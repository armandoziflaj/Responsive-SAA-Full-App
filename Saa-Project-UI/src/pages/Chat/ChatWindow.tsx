import { useRef, useEffect } from 'react';
import styles from './Chat.module.css';
import { type MessagesResponse } from "../../Types/MessagesResponse";
import { type ActiveChat } from "../../Types/ChatTypes";

interface ChatWindowProps {
    activeChat: ActiveChat;
    messages: MessagesResponse[];
    currentUserId: number;
    newMessage: string;
    setNewMessage: (val: string) => void;
    onSendMessage: () => void;
}

const ChatWindow = ({ activeChat, messages, currentUserId, newMessage, setNewMessage, onSendMessage }: ChatWindowProps) => {
    const messagesEndRef = useRef<HTMLDivElement | null>(null);

    useEffect(() => {
        messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
    }, [messages]);

    return (
        <div className={styles.chatWindow}>
            <header className={styles.chatHeader}>
                <h3>{activeChat.name}</h3>
            </header>

            <div className={styles.messageList}>
                <div style={{ marginTop: 'auto' }} />
                {messages.map((m) => {
                    const isMine = m.senderId === currentUserId;
                    return (
                        <div key={m.id} className={isMine ? styles.myMsg : styles.theirMsg}>
                            <p>{m.content}</p>
                            <small>{new Date(m.createdOn).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}</small>
                        </div>
                    );
                })}
                <div ref={messagesEndRef} />
            </div>

            <div className={styles.inputArea}>
                <input
                    value={newMessage}
                    onChange={(e) => setNewMessage(e.target.value)}
                    onKeyDown={(e) => e.key === 'Enter' && onSendMessage()}
                    placeholder="Type a message..."
                />
                <button onClick={onSendMessage}>
                    <i className="fa fa-paper-plane"></i>
                </button>
            </div>
        </div>
    );
};

export default ChatWindow;