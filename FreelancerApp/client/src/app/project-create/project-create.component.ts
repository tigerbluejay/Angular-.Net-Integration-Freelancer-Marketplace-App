
import { ActivatedRoute, Router } from '@angular/router';
import { Member } from '../_models/member';
import { AccountService } from '../_services/account.service';
import { MembersService } from '../_services/members.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { Project } from '../_models/project';
import { Component, OnInit } from '@angular/core';
import { ProjectService } from '../_services/project.service';

@Component({
  selector: 'app-project-create',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './project-create.component.html',
  styleUrl: './project-create.component.css'
})
export class ProjectCreateComponent implements OnInit {
  member: Member | undefined;
  project: Project | null = null;
  isEditMode = false;
  availableSkills: string[] = ['C#', 'ASP.NET Core', 'SQL', 'Angular', 'JavaScript'];


  constructor(
    private activeRouter: ActivatedRoute,
    private router: Router,
    private membersService: MembersService,
    private projectService: ProjectService,
    private accountService: AccountService,
    private toastr: ToastrService     // <--- inject ToastrService
  ) { }

  ngOnInit(): void {
    const id = this.activeRouter.snapshot.paramMap.get('id');
    this.isEditMode = !!id;

    this.loadMemberAndProject(Number(id));
  }

  loadMemberAndProject(id: number) {
    const user = this.accountService.currentUser();
    if (!user) return;

    this.membersService.getMember(user.username).subscribe({
      next: member => {
        this.member = member;

        if (this.isEditMode) {
          this.project = member.clientProjects.find(p => p.id === id) ?? null;

          if (!this.project) {
            this.toastr.error('Project not found.');
          }
        } else {
          this.project = {
            id: 0,
            title: '',
            description: '',
            photoUrl: '',
            isAssigned: false,
            skills: []
          };
        }
      },
      error: err => {
        this.toastr.error('Failed to load member data.');
        console.error(err);
      }
    });
  }

  onSubmit() {
    if (!this.project) return;

    if (this.isEditMode) {
      this.projectService.updateProject(this.project.id, this.project).subscribe({
        next: () => {
          this.toastr.success('Project updated successfully.');
          this.goToProfile();
        },
        error: err => {
          this.toastr.error('Failed to update project.');
          console.error(err);
        }
      });
    } else {
      this.projectService.createProject(this.project).subscribe({
        next: () => {
          this.toastr.success('Portfolio item created successfully.');
          this.goToProfile();
        },
        error: err => {
          this.toastr.error('Failed to create portfolio item.');
          console.error(err);
        }
      });
    }
  }

  goToProfile() {
    if (this.member?.userName) {
      this.router.navigate(['/profile', this.member.userName]);
    }
  }

  onSkillChange(event: Event, skill: string) {
  const checked = (event.target as HTMLInputElement).checked;

  if (checked) {
    if (!this.project?.skills.includes(skill)) {
      this.project?.skills.push(skill);
    }
  } else {
    this.project!.skills = this.project!.skills.filter(s => s !== skill);
  }
}
}
