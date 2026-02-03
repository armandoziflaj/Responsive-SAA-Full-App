export interface MessagesResponse {
    id: number;
    content: string;
    senderId: number;
    receiverId?: number;
    groupId?: number;
    createdOn: string;
}