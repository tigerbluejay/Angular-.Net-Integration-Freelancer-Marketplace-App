import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MembersService } from '../_services/members.service';
import { AccountService } from '../_services/account.service';
import { ProjectConversationDTO } from '../_DTOs/projectConversationDTO';
import { Member } from '../_models/member';
import { ProjectConversationService } from '../_services/project-conversation.service';
import { TimeAgoPipe } from '../_pipes/time-ago.pipe';
import { NgxSpinnerComponent } from "ngx-spinner";

@Component({
  selector: 'app-messages',
  standalone: true,
  imports: [CommonModule, RouterModule, TimeAgoPipe, NgxSpinnerComponent],
  templateUrl: './messages.component.html'
})
export class MessagesComponent implements OnInit {
  private conversationService = inject(ProjectConversationService);
  private memberService = inject(MembersService);
  private accountService = inject(AccountService);

  conversations = signal<ProjectConversationDTO[]>([]);
  currentMember = signal<Member | null>(null);
  loading = signal<boolean>(true);

  ngOnInit(): void {
    const currentUser = this.accountService.currentUser();
    if (!currentUser) {
      console.warn('No current user found!');
      this.loading.set(false);
      return;
    }

    this.memberService.getMember(currentUser.username).subscribe({
      next: member => {
        console.log('Fetched member:', member);
        this.currentMember.set(member);
        this.loadConversations();
      },
      error: err => {
        console.error('Error fetching member:', err);
        this.loading.set(false);
      }
    });
  }

  loadConversations(): void {
    this.loading.set(true);
    console.log('Fetching conversations...');
    this.conversationService.getConversations().subscribe({
      next: convs => {
        console.log('Fetched conversations:', convs);
        this.conversations.set(convs);
        this.loading.set(false);
      },
      error: err => {
        console.error('Error fetching conversations:', err);
        this.loading.set(false);
      }
    });
  }

  getCounterpartPhoto(conv: ProjectConversationDTO): string {
    const member = this.currentMember();
    if (!member) return './assets/user.png';
    return member.id === conv.clientId
      ? conv.freelancerPhotoUrl || './assets/user.png'
      : conv.clientPhotoUrl || './assets/user.png';
  }

  getCounterpartUsername(conv: ProjectConversationDTO): string {
    const member = this.currentMember();
    if (!member) return '';
    return member.id === conv.clientId
      ? conv.freelancerUsername
      : conv.clientUsername;
  }
}