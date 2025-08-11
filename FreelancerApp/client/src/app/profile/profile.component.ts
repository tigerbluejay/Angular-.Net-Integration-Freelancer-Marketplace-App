import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MembersService } from '../_services/members.service';
import { Member } from '../_models/member';
import { DatePipe, NgIf } from '@angular/common';
import { AgePipe } from '../_pipes/age.pipe';
import { PortfolioListComponent } from '../portfolio-list/portfolio-list.component';
import { ProfileProjectListComponent } from '../profile-project-list/profile-project-list.component';
import { PortfolioItemService } from '../_services/portfolio-item.service';
import { PortfolioItem } from '../_models/portfolio-item';
import { ToastrService } from 'ngx-toastr';
import { ProjectService } from '../_services/project.service';

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
  username: string | null = null;

  constructor(route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      this.loadMember();
    });
  }

   loadMember() {
    const username = this.route.snapshot.paramMap.get('username');
    if (!username) return;

    this.memberService.getMember(username).subscribe({
      next: member => this.member = member,
      error: err => console.error('Failed to load member', err)
    });
  }

  onDeletePortfolioItem(id: number) {
    this.portfolioItemService.deletePortfolioItem(id).subscribe({
      next: () => {
        // THIS is what your view is using!
        if (this.member) {
          this.member.portfolioItems = this.member.portfolioItems.filter(item => item.id !== id);
        }
        this.toastr.success('Portfolio item deleted successfully', 'Deleted');
      },
      error: err => {
        this.toastr.error('Failed to delete portfolio item', 'Error');
      }
    });
  }

  onDeleteProject(id: number) {
    this.projectService.deleteProject(id).subscribe({
      next: () => {
        // THIS is what your view is using!
        if (this.member) {
          this.member.clientProjects = this.member.clientProjects.filter(item => item.id !== id);
        }
        this.toastr.success('Project deleted successfully', 'Deleted');
      },
      error: err => {
        this.toastr.error('Failed to delete project', 'Error');
      }
    });
  }

  onEditClientClick() {
    this.router.navigate(['/profile-edit-client'], {
    });
  }

  onEditFreelancerClick() {
    this.router.navigate(['/profile-edit-freelancer'], {
    });
  }




}
