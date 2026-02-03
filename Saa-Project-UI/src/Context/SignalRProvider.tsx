import React, { useEffect, useState, useRef } from 'react';
import * as signalR from "@microsoft/signalr";
import { SignalRContext } from './SignalRContext';
import { type NotificationResponse } from '../Types/NotificationResponse';
import { type MessagesResponse } from '../Types/MessagesResponse';

export const SignalRProvider = ({ children }: { children: React.ReactNode }) => {
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
    const [isJoined, setIsJoined] = useState(false);
    const currentUserId = parseInt(localStorage.getItem('userId') || '0');

    const connectionRef = useRef<signalR.HubConnection | null>(null);

    useEffect(() => {
        if (!currentUserId) return;

        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5245/chatHub", {
                skipNegotiation: true,
                transport: signalR.HttpTransportType.WebSockets
            })
            .configureLogging(signalR.LogLevel.None)
            .withAutomaticReconnect()
            .build();

        const startConnection = async () => {
            try {
                await newConnection.start();
                console.log("✅ SignalR Connected Globally");

                await newConnection.invoke("JoinChat", `private_${currentUserId}`);

                newConnection.on("ReceiveMessage", (message: MessagesResponse) => {
                    console.log("📩 SignalR Message:", message);
                    window.dispatchEvent(new CustomEvent("app-message", { detail: message }));
                });

                newConnection.on("ReceiveNotification", (notif: NotificationResponse) => {
                    console.log("🔔 SignalR Notification:", notif);
                    window.dispatchEvent(new CustomEvent("app-notification", { detail: notif }));
                });

                connectionRef.current = newConnection;
                setConnection(newConnection);
                setIsJoined(true);
            } catch (err) {

                if (err instanceof Error && err.name === 'AbortError') {
                    return;
                }
                console.error("❌ SignalR Connection Error:", err);
            }
        };

        startConnection();

        return () => {
            console.log("🔌 SignalR Cleanup...");
            if (newConnection) {
                newConnection.off("ReceiveMessage");
                newConnection.off("ReceiveNotification");
                newConnection?.stop();
            }
            connectionRef.current = null;
            setConnection(null);
            setIsJoined(false);
        };
    }, [currentUserId]);

    return (
        <SignalRContext.Provider value={{ connection, isJoined }}>
            {children}
        </SignalRContext.Provider>
    );
};