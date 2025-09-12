export interface MessageCreateDTO {
  recipientId: number;
  content: string;
  conversationId: number; // <-- add this
}