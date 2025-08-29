import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountService } from '../_services/account.service';
import { MembersService } from '../_services/members.service';
import { ProposalService } from '../_services/proposal.service';
import { ProposalWithProjectCombinedDTO } from '../_DTOs/proposalWithProjectCombinedDTO';

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

  // Computed signals for each category
  accepted = computed(() =>
    this.proposals().filter(p => p.proposal.isAccepted === true)
  );
  rejected = computed(() =>
    this.proposals().filter(p => p.proposal.isAccepted === false)
  );
  pending = computed(() =>
    this.proposals().filter(p => p.proposal.isAccepted === undefined)
  );

  ngOnInit(): void {
    const currentUser = this.accountService.currentUser();

    if (!currentUser) {
      console.error('No logged-in user found.');
      this.loading.set(false);
      return;
    }

    // fetch Member by username to get the actual ID
    this.membersService.getMember(currentUser.username).subscribe({
      next: member => {
        const freelancerId = member.id;
        this.proposalsService.getProposalsWithProjects(freelancerId).subscribe({
          next: proposals => {
            const normalized = proposals.map(p => ({
              ...p,
              proposal: {
                ...p.proposal,
                isAccepted: this.normalizeIsAccepted(p.proposal.isAccepted)
              }
            }));
            this.proposals.set(normalized);
            this.loading.set(false);  // ✅ mark loading complete
          },
          error: err => console.error('Error fetching proposals:', err)
        });
      },
      error: err => console.error('Error fetching member:', err)
    });
  }

  setTab(tab: 'accepted' | 'rejected' | 'pending') {
    this.activeTab.set(tab);
  }

  // Helper to safely normalize isAccepted to boolean | undefined
  private normalizeIsAccepted(value: boolean | undefined | number): boolean | undefined {
    if (value === true || value === 1) return true;
    if (value === false || value === 0) return false;
    return undefined;
  }
}