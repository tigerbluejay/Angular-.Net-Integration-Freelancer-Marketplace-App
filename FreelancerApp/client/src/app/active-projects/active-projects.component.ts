import { Component, OnInit, inject, signal } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { MembersService } from '../_services/members.service';
import { ProjectService } from '../_services/project.service';
import { Member } from '../_models/member';
import { ProjectActiveDTO } from '../_DTOs/projectActiveDTO';
import { CommonModule, JsonPipe } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-active-projects',
  templateUrl: './active-projects.component.html',
  styleUrls: ['./active-projects.component.css'],
  imports: [JsonPipe, CommonModule]
})
export class ActiveProjectsComponent implements OnInit {
  private accountService = inject(AccountService);
  private membersService = inject(MembersService);
  private projectService = inject(ProjectService);

  projects = signal<ProjectActiveDTO[]>([]);
  loading = signal<boolean>(true);
  hasMoreProjects = signal<boolean>(true);

  pageNumber = 1;
  pageSize = 6;

  role: 'Client' | 'Freelancer' | 'Admin' | null = null;
  userId: number | null = null;

  ngOnInit(): void {
    const user = this.accountService.currentUser();
    if (!user) {
      this.loading.set(false);
      return;
    }

    // Fetch full member profile to get ID + roles
    this.membersService.getMember(user.username).subscribe({
      next: (member: Member) => {
        this.userId = member.id;
        if (member.roles.includes('Client')) {
          this.role = 'Client';
          this.loadClientProjects();
        } else if (member.roles.includes('Freelancer')) {
          this.role = 'Freelancer';
          this.loadFreelancerProjects();
        } else if (member.roles.includes('Admin')) {
          this.role = 'Admin';
          // 👇 Decide admin logic (e.g., show all projects)
          this.loadClientProjects();
        }
      },
      error: () => this.loading.set(false)
    });
  }


private loadClientProjects() {
  if (!this.userId) return;

  this.projectService
    .getProjectsByClientId(this.userId, this.pageNumber, this.pageSize)
    .subscribe({
      next: (projects) => {
        this.projects.set(projects);
        this.hasMoreProjects.set(projects.length === this.pageSize);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
}
  private loadFreelancerProjects() {
    if (!this.userId) return;

    this.projectService
      .getProjectsByFreelancerId(this.userId, this.pageNumber, this.pageSize).subscribe({
        next: (projects) => {
          this.projects.set(projects);
          this.hasMoreProjects.set(projects.length === this.pageSize);
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
  }

  nextPage() {
    this.pageNumber++;
    this.reloadProjects();
  }

  previousPage() {
    if (this.pageNumber > 1) {
      this.pageNumber--;
      this.reloadProjects();
    }
  }

  private reloadProjects() {
    if (this.role === 'Client') {
      this.loadClientProjects();
    } else if (this.role === 'Freelancer') {
      this.loadFreelancerProjects();
    } else if (this.role === 'Admin') {
      this.loadClientProjects(); // or a different admin-specific loader
    }
  }
}
