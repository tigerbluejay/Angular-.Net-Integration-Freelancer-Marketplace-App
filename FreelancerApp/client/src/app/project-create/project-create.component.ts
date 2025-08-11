
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
import { PhotoService } from '../_services/photo.service';

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

  selectedPhotoFile?: File;
  photoPreviewUrl: string | null | undefined;

  constructor(
    private activeRouter: ActivatedRoute,
    private router: Router,
    private membersService: MembersService,
    private projectService: ProjectService,
    private accountService: AccountService,
    private photoService: PhotoService,
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
          } else {
            this.photoPreviewUrl = this.project.photoUrl || undefined;
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
          this.photoPreviewUrl = undefined;
        }
      },
      error: err => {
        this.toastr.error('Failed to load member data.');
        console.error(err);
      }
    });
  }
  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedPhotoFile = input.files[0];

      const reader = new FileReader();
      reader.onload = () => {
        this.photoPreviewUrl = reader.result as string;
      };
      reader.readAsDataURL(this.selectedPhotoFile);
    }
  }

  onDeletePhoto() {
  if (!confirm('Are you sure you want to delete this photo?')) return;

  if (!this.project) return;

  this.photoService.deleteProjectPhoto(this.project.id).subscribe({
    next: () => {
      this.toastr.success('Photo deleted successfully');
      this.photoPreviewUrl = undefined;
      this.selectedPhotoFile = undefined;
      this.project!.photoUrl = '';

      // Optionally mark form dirty or trigger UI update here
    },
    error: err => {
      this.toastr.error('Failed to delete photo');
      console.error(err);
    }
  });
}


  onSubmit() {
    if (!this.project) return;

    if (this.isEditMode) {
      // Editing existing project
      if (this.selectedPhotoFile) {
        // Upload photo first
        this.photoService.uploadProjectPhoto(this.project.id, this.selectedPhotoFile).subscribe({
          next: photo => {
            this.toastr.success('Photo uploaded successfully');
            if (photo.url) {
              this.project!.photoUrl = photo.url;
            }
            // Then update project with photoUrl
            this.saveProject();
          },
          error: err => {
            console.error('Photo upload failed:', err);
            this.toastr.error('Failed to upload photo');
          }
        });
      } else {
        // Just update project without photo upload
        this.saveProject();
      }
    } else {
      // Creating new project
      this.projectService.createProject(this.project).subscribe({
        next: createdProject => {
          this.toastr.success('Project created successfully');

          if (this.selectedPhotoFile) {
            // Upload photo now with the new projectId
            this.photoService.uploadProjectPhoto(createdProject.id, this.selectedPhotoFile).subscribe({
              next: photo => {
                if (photo.url) {
                  createdProject.photoUrl = photo.url;
                }
                // Update the project again with the photoUrl
                this.projectService.updateProject(createdProject.id, createdProject).subscribe({
                  next: () => this.goToProfile(),
                  error: err => {
                    this.toastr.error('Failed to update project with photo.');
                    console.error(err);
                  }
                });
              },
              error: err => {
                this.toastr.error('Failed to upload photo');
                console.error(err);
              }
            });
          } else {
            // No photo selected, just go to profile
            this.goToProfile();
          }
        },
        error: err => {
          this.toastr.error('Failed to create project.');
          console.error(err);
        }
      });
    }
  }

  private saveProject() {
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
          this.toastr.success('Project created successfully.');
          this.goToProfile();
        },
        error: err => {
          this.toastr.error('Failed to create project.');
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
