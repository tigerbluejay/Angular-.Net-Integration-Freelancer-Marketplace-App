import { Component, inject, OnInit, signal } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ProposalService } from '../_services/proposal.service';
import { MembersService } from '../_services/members.service';
import { ProposalWithProjectCombinedDTO } from '../_DTOs/proposalWithProjectCombinedDTO';
import { ProjectAssignDTO } from '../_DTOs/projectAssignDTO';
import { ProposalAssignDTO } from '../_DTOs/proposalAssignDTO';
import { PaginatedResult2 } from '../_models/pagination2';
import { CommonModule } from '@angular/common';
import { ProjectService } from '../_services/project.service';
import { ProposalwithProjectAssignCombinedDTO } from '../_DTOs/proposalwithProjectAssignCombinedDTO_tmp';

@Component({
  selector: 'app-proposal-inbox',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './proposal-inbox.component.html',
  styleUrl: './proposal-inbox.component.css'
})
export class ProposalInboxComponent implements OnInit {
  private proposalsService = inject(ProposalService);
  private projectService = inject(ProjectService);
  private accountService = inject(AccountService);
  private membersService = inject(MembersService);

  proposals = signal<ProposalWithProjectCombinedDTO[]>([]);
  loading = signal(true);

  currentPage = signal(1);
  totalPages = signal(1);
  pageSize = 10;

  ngOnInit(): void {
    this.loadProposals();
  }

  loadProposals() {
    const currentUser = this.accountService.currentUser();
    if (!currentUser) {
      console.error('No logged-in user found.');
      this.loading.set(false);
      return;
    }

    this.loading.set(true);

    this.membersService.getMember(currentUser.username).subscribe({
      next: member => {
        const clientId = member.id;
        const page = this.currentPage();

        this.proposalsService.getInboxProposalsForClient(
          clientId,
          page,
          this.pageSize
        ).subscribe({
          next: (paginatedResult: PaginatedResult2<ProposalWithProjectCombinedDTO>) => {
            this.proposals.set(paginatedResult.result);
            this.totalPages.set(paginatedResult.pagination!.totalPages);
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
    if (this.currentPage() < this.totalPages()) {
      this.currentPage.set(this.currentPage() + 1);
      this.loadProposals();
    }
  }

  prevPage() {
    if (this.currentPage() > 1) {
      this.currentPage.set(this.currentPage() - 1);
      this.loadProposals();
    }
  }

  assignProposal(item: ProposalWithProjectCombinedDTO, accept: boolean) {
    const dto: ProposalwithProjectAssignCombinedDTO = {
      proposal: {
        id: item.proposal.id,
        isAccepted: accept
      } as ProposalAssignDTO,
      project: {
        id: item.project.id,
        freelancerUserId: item.proposal.freelancerUserId 
      } as ProjectAssignDTO
    };

    this.projectService.patchProposalWithProjectAssign(dto).subscribe({
      next: (response) => {
        console.log('Assignment saved:', response);
        // Optional: refresh list or update UI state
        this.loadProposals();
      },
      error: (err) => {
        console.error('Error assigning proposal:', err);
      }
    });
  }
}