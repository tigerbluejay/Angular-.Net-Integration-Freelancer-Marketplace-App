import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MembersService } from '../_services/members.service';
import { Member } from '../_models/member';
import { DatePipe, NgIf } from '@angular/common';
import { AgePipe } from '../_pipes/age.pipe';
import { PortfolioListComponent } from '../portfolio-list/portfolio-list.component';
import { ProfileProjectListComponent } from '../profile-project-list/profile-project-list.component';
import { PortfolioItemService } from '../_services/portfolio-item.service';
import { ProjectService } from '../_services/project.service';
import { ToastrService } from 'ngx-toastr';
import { PortfolioItemDTO } from '../_DTOs/portfolioItemDTO';
import { ProjectDTO } from '../_DTOs/projectDTO';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [RouterModule, NgIf, DatePipe, AgePipe, PortfolioListComponent, ProfileProjectListComponent],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit {

  private memberService = inject(MembersService);
  private portfolioItemService = inject(PortfolioItemService);
  private projectService = inject(ProjectService);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);
  private router = inject(Router);

  member?: Member;

  // Lists
  portfolio: PortfolioItemDTO[] = [];
  projects: ProjectDTO[] = [];

  // Pagination state
  portfolioPageNumber = 1;
  projectPageNumber = 1;
  pageSize = 6;

  totalPortfolioItems = 0;
  totalProjects = 0;

  ngOnInit(): void {
    this.route.paramMap.subscribe(() => {
      this.loadMember();
    });
  }

  loadMember() {
    const username = this.route.snapshot.paramMap.get('username');
    if (!username) return;

    this.memberService.getMember(username).subscribe({
      next: member => {
        this.member = member;

        // Load data based on role
        if (member.roles?.includes('Freelancer')) this.loadPortfolio();
        if (member.roles?.includes('Client')) this.loadProjects();
      },
      error: err => console.error('Failed to load member', err)
    });
  }

  // ----------------- Portfolio -----------------
  loadPortfolio() {
    if (!this.member) return;

    this.memberService.getFreelancerPortfolio(this.member.userName, this.portfolioPageNumber, this.pageSize)
      .subscribe(response => {
        this.portfolio = response.body!;
        const paginationHeader = response.headers.get('Pagination');
        if (paginationHeader) {
          const pagination = JSON.parse(paginationHeader);
          this.totalPortfolioItems = pagination.totalItems;
        }
      });
  }

  portfolioTotalPages(): number {
    return Math.ceil(this.totalPortfolioItems / this.pageSize);
  }

  portfolioPages(): number[] {
    return Array.from({ length: this.portfolioTotalPages() }, (_, i) => i + 1);
  }

  changePortfolioPage(page: number) {
    if (page < 1 || page > this.portfolioTotalPages()) return;
    this.portfolioPageNumber = page;
    this.loadPortfolio();
  }

  onDeletePortfolioItem(id: number) {
    this.portfolioItemService.deletePortfolioItem(id).subscribe({
      next: () => {
        this.portfolio = this.portfolio.filter(item => item.id !== id);
        this.toastr.success('Portfolio item deleted successfully', 'Deleted');
      },
      error: () => this.toastr.error('Failed to delete portfolio item', 'Error')
    });
  }

  // ----------------- Projects -----------------
  loadProjects() {
    if (!this.member) return;

    this.memberService.getClientProjects(this.member.userName, this.projectPageNumber, this.pageSize)
      .subscribe(response => {
        this.projects = response.body!;
        const paginationHeader = response.headers.get('Pagination');
        if (paginationHeader) {
          const pagination = JSON.parse(paginationHeader);
          this.totalProjects = pagination.totalItems;
        }
      });
  }

  projectTotalPages(): number {
    return Math.ceil(this.totalProjects / this.pageSize);
  }

  projectPages(): number[] {
    return Array.from({ length: this.projectTotalPages() }, (_, i) => i + 1);
  }

  changeProjectPage(page: number) {
    if (page < 1 || page > this.projectTotalPages()) return;
    this.projectPageNumber = page;
    this.loadProjects();
  }

  onDeleteProject(id: number) {
    this.projectService.deleteProject(id).subscribe({
      next: () => {
        this.projects = this.projects.filter(item => item.id !== id);
        this.toastr.success('Project deleted successfully', 'Deleted');
      },
      error: () => this.toastr.error('Failed to delete project', 'Error')
    });
  }

  // ----------------- Navigation -----------------
  onEditClientClick() {
    this.router.navigate(['/profile-edit-client']);
  }

  onEditFreelancerClick() {
    this.router.navigate(['/profile-edit-freelancer']);
  }
}