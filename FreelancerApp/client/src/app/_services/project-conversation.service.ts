import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MessageCreateDTO } from '../_DTOs/messageCreateDTO';
import { MessageDTO } from '../_DTOs/messageDTO';
import { ProjectConversationDTO } from '../_DTOs/projectConversationDTO';
import { environment } from '../../environments/environment';


@Injectable({
  providedIn: 'root',
})

export class ProjectConversationService {
  baseUrl = environment.apiUrl; // e.g. "https://localhost:5001/api/"

  constructor(private http: HttpClient) {}

  /**
   * Send a message in a project conversation.
   * @param projectId The project ID
   * @param dto The message data
   */
  sendMessage(projectId: number, dto: MessageCreateDTO): Observable<MessageDTO> {
    return this.http.post<MessageDTO>(`${this.baseUrl}projectconversation/${projectId}`, dto);
  }

  /**
   * Get all messages for a given project conversation.
   * @param projectId The project ID
   * @param freelancerId The freelancer ID
   */
  getMessages(
    projectId: number,
    freelancerId: number
  ): Observable<MessageDTO[]> {
    return this.http.get<MessageDTO[]>(
      `${this.baseUrl}projectconversation/${projectId}/${freelancerId}`
    );
  }

  /**
   * Get all project conversations for the current user.
   */
  getConversations(): Observable<ProjectConversationDTO[]> {
    return this.http.get<ProjectConversationDTO[]>(
      `${this.baseUrl}projectconversation/conversations`
    );
  }
}