import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProjectConversationService } from '../_services/project-conversation.service';
import { MembersService } from '../_services/members.service';
import { AccountService } from '../_services/account.service';
import { MessageDTO } from '../_DTOs/messageDTO';
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

  projectId!: number;
  conversationId!: number;
  freelancerId!: number;

  projectTitle = signal<string>('Project'); // default

 ngOnInit(): void {
  this.projectId = Number(this.route.snapshot.paramMap.get('projectId'));
  this.conversationId = Number(this.route.snapshot.queryParamMap.get('conversationId'));
  const freelancerId = Number(this.route.snapshot.queryParamMap.get('freelancerId'));

  const currentUser = this.accountService.currentUser();
  if (!currentUser) {
    console.error('No current user found!');
    this.loading.set(false);
    return;
  }

  // Fetch full member info including id
  this.memberService.getMember(currentUser.username).subscribe({
    next: member => {
      this.currentMember.set(member);

      // Optionally, fetch conversation info to get projectTitle
      this.conversationService.getConversations().subscribe({
        next: convs => {
          const conv = convs.find(c => c.projectId === this.projectId && c.freelancerId === freelancerId);
          if (conv) this.projectTitle.set(conv.projectTitle);
        },
        error: err => console.error('Error fetching project title:', err)
      });

      this.loadMessages(freelancerId);
    },
    error: err => {
      console.error('Error fetching member:', err);
      this.loading.set(false);
    }
  });
}

loadMessages(freelancerId: number): void {
  this.loading.set(true);

  this.conversationService.getMessages(this.projectId, freelancerId).subscribe({
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

    this.conversationService.sendMessage(this.projectId, {
      recipientId: this.getRecipientId(),
      content
    }).subscribe({
      next: msg => {
        // update messages array immutably
        this.messages.update(curr => [...curr, msg]);
        this.newMessage.set('');
      },
      error: err => console.error('Error sending message:', err)
    });
  }

  getRecipientId(): number {
    const msgs = this.messages();
    if (!msgs.length) return 0;
    const firstMsg = msgs[0];
    const member = this.currentMember();
    if (!member) return 0;

    return firstMsg.senderId === member.id ? firstMsg.recipientId : firstMsg.senderId;
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