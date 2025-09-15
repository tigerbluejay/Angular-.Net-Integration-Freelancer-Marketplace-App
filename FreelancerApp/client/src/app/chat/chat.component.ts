import { Component, OnInit, signal, inject, OnDestroy, AfterViewChecked, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ProjectConversationService } from '../_services/project-conversation.service';
import { MembersService } from '../_services/members.service';
import { AccountService } from '../_services/account.service';
import { MessageDTO } from '../_DTOs/messageDTO';
import { ProjectConversationDTO } from '../_DTOs/projectConversationDTO';
import { Member } from '../_models/member';
import { NgxSpinnerComponent } from 'ngx-spinner';
import { MessageCreateDTO } from '../_DTOs/messageCreateDTO';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule, NgxSpinnerComponent],
  templateUrl: './chat.component.html',
  styles: [`
    .trash-btn {
      position: absolute;
      top: 2px;
      right: 2px;
      font-size: 0.8rem;
      color: rgba(255,255,255,0.7);
      border: none;
      background: transparent;
      cursor: pointer;
      padding: 0;
      transition: color 0.2s;
    }
    .trash-btn:hover { color: rgba(255,255,255,1); }
  `]
})
export class ChatComponent implements OnInit, OnDestroy, AfterViewChecked {

  @ViewChild('scrollMe') scrollContainer?: any;


  private conversationService = inject(ProjectConversationService);
  private memberService = inject(MembersService);
  private accountService = inject(AccountService);
  private route = inject(ActivatedRoute);

  messages = this.conversationService.messageThread;
  loading = signal<boolean>(true);
  newMessage = signal<string>('');
  currentMember = signal<Member | null>(null);
  projectConversation = signal<ProjectConversationDTO | null>(null);

  hoveredMessageId = signal<number | null>(null);

  ngOnInit(): void {
    const conversationId = Number(this.route.snapshot.queryParamMap.get('conversationId'));
    if (!conversationId) { console.error('Conversation ID missing'); this.loading.set(false); return; }

    const currentUser = this.accountService.currentUser();
    if (!currentUser) { console.error('No current user'); this.loading.set(false); return; }

    this.memberService.getMember(currentUser.username).subscribe({
      next: member => {
        this.currentMember.set(member);
        this.conversationService.getConversations().subscribe({
          next: convs => {
            const conv = convs.find(c => c.conversationId === conversationId);
            if (!conv) { console.error('Conversation not found'); this.loading.set(false); return; }

            this.projectConversation.set(conv);
            this.loadMessages(conv);
            console.log('[ChatComponent] Starting SignalR hub connection...');
            this.conversationService.createHubConnection(currentUser, conv.conversationId);
          },
          error: err => { console.error('Error fetching conversations', err); this.loading.set(false); }
        });
      },
      error: err => { console.error('Error fetching member', err); this.loading.set(false); }
    });
  }

  loadMessages(conv: ProjectConversationDTO): void {
    const member = this.currentMember();
    if (!member) return;

    this.conversationService.getMessages(conv.projectId, conv.freelancerId).subscribe({
      next: msgs => {
        console.log('[ChatComponent] Loaded messages', msgs);
        this.conversationService.messageThread.set(msgs);
        this.loading.set(false);
      },
      error: err => { console.error('Error loading messages', err); this.loading.set(false); }
    });
  }

  sendMessage(): void {
    const content = this.newMessage();
    if (!content.trim()) return;

    const conv = this.projectConversation();
    if (!conv) return;

    const member = this.currentMember();
    if (!member) return;

    const recipientId = member.id === conv.clientId ? conv.freelancerId : conv.clientId;

    const dto: MessageCreateDTO = {
      recipientId,
      content,
      conversationId: conv.conversationId
    };

    console.log('[ChatComponent] Sending message DTO:', dto);

    this.conversationService.sendMessageSignalR(dto)
      .then(() => this.newMessage.set(''))
      .catch(err => {
        console.error('SignalR send failed, falling back to HTTP:', err);
        this.conversationService.sendMessage(conv.projectId, dto).subscribe({
          next: msg => { this.messages.update(curr => [...curr, msg]); this.newMessage.set(''); },
          error: e => console.error('HTTP fallback failed:', e)
        });
      });
  }

  ngOnDestroy(): void {
    this.conversationService.stopHubConnection();
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

  onDeleteMessage(messageId: number): void {
    if (!confirm('Are you sure you want to delete this message? Other users in this conversation will still see it.')) return;

    this.conversationService.deleteMessage(messageId).subscribe({
      next: () => {
        this.messages.update(curr => curr.filter(m => m.id !== messageId));
      },
      error: err => {
        console.error('Error deleting message:', err);
        alert('Failed to delete message. Please try again.');
      }
    });
  }

  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }

  scrollToBottom() {
    if (this.scrollContainer)
      this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
  }
}