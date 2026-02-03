export interface ActiveChat {
    type: 'private' | 'group';
    id: number;
    name: string;
}

export interface NotificationPayload {
    message: string;
    type: "PrivateMessage" | "GroupMessage";
    fromId?: number;
    groupId?: number;
}
