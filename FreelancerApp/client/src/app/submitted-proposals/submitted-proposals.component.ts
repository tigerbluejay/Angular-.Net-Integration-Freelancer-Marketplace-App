import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountService } from '../_services/account.service';
import { MembersService } from '../_services/members.service';
import { ProposalService } from '../_services/proposal.service';
import { ProposalWithProjectCombinedDTO } from '../_DTOs/proposalWithProjectCombinedDTO';
import { PaginatedResult, Pagination } from '../_models/pagination';
import { PaginatedResult2 } from '../_models/pagination2';

@Component({
  selector: 'app-submitted-proposals',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './submitted-proposals.component.html',
  styleUrls: ['./submitted-proposals.component.css']
})
export class SubmittedProposalsComponent implements OnInit {
  private proposalsService = inject(ProposalService);
  private accountService = inject(AccountService);
  private membersService = inject(MembersService);

  // Signals
  proposals = signal<ProposalWithProjectCombinedDTO[]>([]);
  activeTab = signal<'accepted' | 'rejected' | 'pending'>('accepted');
  loading = signal(true);

  // Pagination signals per tab
  currentPage = signal<{ [key in 'accepted' | 'rejected' | 'pending']: number }>({
    accepted: 1,
    rejected: 1,
    pending: 1
  });

  totalPages = signal<{ [key in 'accepted' | 'rejected' | 'pending']: number }>({
    accepted: 1,
    rejected: 1,
    pending: 1
  });

  pageSize = 10; // items per page

  ngOnInit(): void {
    this.loadProposals(this.activeTab());
  }

  setTab(tab: 'accepted' | 'rejected' | 'pending') {
    this.activeTab.set(tab);
    // If first time loading this tab, start at page 1
    if (this.currentPage()[tab] === 1) {
      this.loadProposals(tab);
    } else {
      this.loadProposals(tab);
    }
  }

  loadProposals(tab: 'accepted' | 'rejected' | 'pending') {
    const currentUser = this.accountService.currentUser();
    console.log('Current user:', currentUser);
    if (!currentUser) {
      console.error('No logged-in user found.');
      this.loading.set(false);
      return;
    }

    this.loading.set(true);

    this.membersService.getMember(currentUser.username).subscribe({
      next: member => {
        console.log('Member fetched:', member);
        const freelancerId = member.id;
        const page = this.currentPage()[tab];

        console.log('Loading proposals', {
          freelancerId,
          tab,
          page: this.currentPage()[tab],
          pageSize: this.pageSize
        });
        this.proposalsService.getProposalsWithProjects(freelancerId, page, this.pageSize, tab)
          .subscribe({
            next: (paginatedResult: PaginatedResult2<ProposalWithProjectCombinedDTO>) => {
              console.log('Paginated result:', paginatedResult);
              this.proposals.set(paginatedResult.result);
              this.totalPages()[tab] = paginatedResult.pagination.totalPages;
              this.loading.set(false);
            },
            error: err => {
              console.error(err);
              this.loading.set(false);
            }
          });
      },
      error: err => {
        console.error(err);
        this.loading.set(false);
      }
    });
  }

  nextPage() {
    const tab = this.activeTab();
    if (this.currentPage()[tab] < this.totalPages()[tab]) {
      this.currentPage()[tab]++;
      this.loadProposals(tab);
    }
  }

  prevPage() {
    const tab = this.activeTab();
    if (this.currentPage()[tab] > 1) {
      this.currentPage()[tab]--;
      this.loadProposals(tab);
    }
  }
}