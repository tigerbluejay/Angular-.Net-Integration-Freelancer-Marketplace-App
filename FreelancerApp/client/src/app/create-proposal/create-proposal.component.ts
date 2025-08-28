import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProposalService } from '../_services/proposal.service';
import { ProposalCreateDTO } from '../_DTOs/proposalCreateDTO';
import { ProjectBrowseDTO } from '../_DTOs/projectBrowseDTO';
import { ProjectService } from '../_services/project.service';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MembersService } from '../_services/members.service';
import { AccountService } from '../_services/account.service';
import { Member } from '../_models/member';
import { User } from '../_models/user';
import { Project } from '../_models/project';

@Component({
  selector: 'app-proposal-create',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './create-proposal.component.html',
  styleUrls: ['./create-proposal.component.css']
})
export class ProposalCreateComponent implements OnInit {

  project: ProjectBrowseDTO | null = null; // ✅ Correct type
  model: ProposalCreateDTO = {
    title: '',
    description: '',
    bid: 0, // NEW FIELD
    projectId: 0,
    freelancerUserId: 0,
    clientUserId: 0,
    photoFile: undefined
  };

  photoPreviewUrl: string | null = null;
  currentUser = signal<User | null>(null);

  private memberService = inject(MembersService);
  private projectService = inject(ProjectService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private proposalService = inject(ProposalService);
  private toastr = inject(ToastrService);
  private accountService = inject(AccountService);

  ngOnInit(): void {
    // 1️⃣ Get projectId from route param
    const projectId = +this.route.snapshot.paramMap.get('id')!;
    console.log('Project ID from route:', projectId);
    if (!projectId) {
      this.toastr.error('No project selected.');
      this.router.navigate(['/browse-projects']);
      return;
    }

    // 2️⃣ Fetch project from backend
    this.projectService.getProjectById(projectId).subscribe({
      next: (project: ProjectBrowseDTO) => {
        this.project = project;
        this.model.projectId = project.id;
        this.model.clientUserId = project.clientUserId;
        // 🔍 Log the client photo URL
        console.log('Client photo URL:', project.clientPhotoUrl);
      },
      error: err => {
        console.error(err);
        this.toastr.error('Failed to load project.');
        this.router.navigate(['/browse-projects']);
      }
    });

    // 3️⃣ Fetch freelancer ID
    const user = this.accountService.currentUser();
    if (user?.username) {
      this.memberService.getMember(user.username).subscribe({
        next: member => this.model.freelancerUserId = member.id,
        error: err => console.error(err)
      });
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.model.photoFile = input.files[0];
      const reader = new FileReader();
      reader.onload = () => this.photoPreviewUrl = reader.result as string;
      reader.readAsDataURL(input.files[0]);
    }
  }

  onDeletePhoto(): void {
    this.model.photoFile = undefined;
    this.photoPreviewUrl = null;
  }

  onSubmit(form: any): void {
    if (form.invalid) {
      this.toastr.error('Please fix the form errors before submitting.');
      return;
    }

    this.proposalService.createProposal(this.model).subscribe({
      next: () => {
        this.toastr.success('Proposal submitted successfully!');
        this.router.navigate(['/browse-projects']);
      },
      error: err => {
        console.error(err);
        this.toastr.error('Failed to submit proposal.');
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/browse-projects']);
  }
}