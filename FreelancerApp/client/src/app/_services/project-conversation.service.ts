import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { MessageCreateDTO } from '../_DTOs/messageCreateDTO';
import { MessageDTO } from '../_DTOs/messageDTO';
import { ProjectConversationDTO } from '../_DTOs/projectConversationDTO';
import { User } from '../_models/user';

@Injectable({ providedIn: 'root' })
export class ProjectConversationService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubsUrl;
  private http = inject(HttpClient);
  private hubConnection?: HubConnection;

  messageThread = signal<MessageDTO[]>([]);

  createHubConnection(user: User, conversationId: number) {
    console.log('[ProjectConversationService] Creating hub for conversation:', conversationId);

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?conversationId=' + conversationId, { accessTokenFactory: () => user.token })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .then(() => console.log('[ProjectConversationService] Hub connected'))
      .catch(err => console.error('[ProjectConversationService] Hub start error:', err));

    this.hubConnection.on('ReceiveMessageThread', (messages: MessageDTO[]) => {
      console.log('[ProjectConversationService] Received message thread:', messages);
      this.messageThread.set(messages);
    });

    this.hubConnection.on('NewMessage', (message: MessageDTO) => {
      console.log('[ProjectConversationService] Received new message:', message);
      this.messageThread.update(curr => [...curr, message]);
    });
  }

  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection.stop()
        .then(() => console.log('[ProjectConversationService] Hub stopped'))
        .catch(err => console.error('[ProjectConversationService] Error stopping hub:', err));
    }
  }

  async sendMessageSignalR(dto: MessageCreateDTO) {
    console.log('[ProjectConversationService] Invoking SendMessage via SignalR:', dto);
    return this.hubConnection?.invoke('SendMessage', dto);
  }

  sendMessage(projectId: number, dto: MessageCreateDTO) {
    return this.http.post<MessageDTO>(`${this.baseUrl}projectconversation/${projectId}`, dto);
  }

  getMessages(projectId: number, freelancerId: number) {
    return this.http.get<MessageDTO[]>(`${this.baseUrl}projectconversation/${projectId}/${freelancerId}`);
  }

  getConversations() {
    return this.http.get<ProjectConversationDTO[]>(`${this.baseUrl}projectconversation/conversations`);
  }

  deleteMessage(messageId: number) {
    return this.http.delete<void>(`${this.baseUrl}projectconversation/${messageId}`);
  }
}