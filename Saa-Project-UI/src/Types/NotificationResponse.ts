export interface NotificationResponse {
    id: number;
    message: string;
    type: string;
    relatedId: number;
    userId: number;
    isRead: boolean;
    createdOn: string;
}