export interface MessageDTO {
  id: number;
  senderId: number;
  senderUsername: string;
  senderPhotoUrl: string;
  recipientId: number;
  recipientUsername: string;
  recipientPhotoUrl: string;
  content: string;
  dateRead?: string; // optional ISO string
  messageSent: string; // ISO string
  conversationId: number;
}