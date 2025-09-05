export interface ProjectConversationDTO {
  conversationId: number;
  projectId: number;
  projectTitle: string;

  clientId: number;
  clientUsername: string;
  clientPhotoUrl: string;

  freelancerId: number;
  freelancerUsername: string;
  freelancerPhotoUrl: string;

  lastMessage: string;
  lastMessageSent: string; // ISO string
  unreadCount: number;
}