import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProjectConversationService } from '../_services/project-conversation.service';
import { MembersService } from '../_services/members.service';
import { AccountService } from '../_services/account.service';
import { MessageDTO } from '../_DTOs/messageDTO';
import { ProjectConversationDTO } from '../_DTOs/projectConversationDTO';
import { Member } from '../_models/member';
import { NgxSpinnerComponent } from 'ngx-spinner';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule, NgxSpinnerComponent],
  templateUrl: './chat.component.html'
})
export class ChatComponent implements OnInit {
  private conversationService = inject(ProjectConversationService);
  private memberService = inject(MembersService);
  private accountService = inject(AccountService);
  private route = inject(ActivatedRoute);

  messages = signal<MessageDTO[]>([]);
  loading = signal<boolean>(true);
  newMessage = signal<string>('');
  currentMember = signal<Member | null>(null);
  projectConversation = signal<ProjectConversationDTO | null>(null);

  ngOnInit(): void {
    const conversationId = Number(this.route.snapshot.queryParamMap.get('conversationId'));
    if (!conversationId) {
      console.error('Conversation ID is missing from route params!');
      this.loading.set(false);
      return;
    }

    const currentUser = this.accountService.currentUser();
    if (!currentUser) {
      console.error('No current user found!');
      this.loading.set(false);
      return;
    }

    // Fetch full member info
    this.memberService.getMember(currentUser.username).subscribe({
      next: member => {
        this.currentMember.set(member);

        // Get all conversations and pick the current one
        this.conversationService.getConversations().subscribe({
          next: convs => {
            const conv = convs.find(c => c.conversationId === conversationId);
            if (!conv) {
              console.error('Conversation not found!');
              this.loading.set(false);
              return;
            }

            this.projectConversation.set(conv);
            this.loadMessages(conv);
          },
          error: err => {
            console.error('Error loading conversations:', err);
            this.loading.set(false);
          }
        });
      },
      error: err => {
        console.error('Error fetching member:', err);
        this.loading.set(false);
      }
    });
  }

  loadMessages(conv: ProjectConversationDTO): void {
    const member = this.currentMember();
    if (!member) {
      console.error('Current member not loaded yet!');
      this.loading.set(false);
      return;
    }

    // Determine recipient ID
    const freelancerId = conv.freelancerId;
    this.conversationService.getMessages(conv.projectId, freelancerId).subscribe({
      next: msgs => {
        this.messages.set(msgs);
        this.loading.set(false);
      },
      error: err => {
        console.error('Error loading messages:', err);
        this.loading.set(false);
      }
    });
  }

  sendMessage(): void {
    const content = this.newMessage();
    if (!content.trim()) return;

    const conv = this.projectConversation();
    if (!conv) return;

    const member = this.currentMember();
    if (!member) return;

    // Determine recipient
    const recipientId = member.id === conv.clientId ? conv.freelancerId : conv.clientId;

    this.conversationService.sendMessage(conv.projectId, { recipientId, content }).subscribe({
      next: msg => {
        this.messages.update(curr => [...curr, msg]);
        this.newMessage.set('');
      },
      error: err => console.error('Error sending message:', err)
    });
  }

  getUserPhoto(msg: MessageDTO): string {
    const member = this.currentMember();
    if (!member) return './assets/user.png';
    return msg.senderId === member.id ? member.photoUrl || './assets/user.png' : msg.senderPhotoUrl;
  }

  getUserName(msg: MessageDTO): string {
    const member = this.currentMember();
    if (!member) return 'You';
    return msg.senderId === member.id ? member.userName || 'You' : msg.senderUsername;
  }
}