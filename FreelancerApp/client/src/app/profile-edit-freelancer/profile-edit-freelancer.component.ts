import { Component, HostListener, inject, OnInit, ViewChild } from '@angular/core';
import { Member } from '../_models/member';
import { AccountService } from '../_services/account.service';
import { MembersService } from '../_services/members.service';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-profile-edit-freelancer',
  templateUrl: './profile-edit-freelancer.component.html',
  styleUrls: ['./profile-edit-freelancer.component.css'],
  imports: [FormsModule]
})
export class ProfileEditFreelancerComponent implements OnInit {
  @ViewChild('editForm') editForm?: NgForm;

  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.editForm?.dirty) {
      $event.returnValue = true;
    }
  }

  member?: Member;

  private accountService = inject(AccountService);
  private membersService = inject(MembersService);
  private toastr = inject(ToastrService);
  router = inject(Router);

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    const user = this.accountService.currentUser();
    if (!user) return;

    this.membersService.getMember(user.username).subscribe({
      next: member => this.member = member,
      error: err => this.toastr.error('Failed to load profile')
    });
  }

  updateMember() {
    if (!this.member) return;

    this.membersService.updateMember(this.member).subscribe({
      next: () => {
        this.toastr.success('Profile updated successfully');
        this.editForm?.reset(this.member); // Reset form state to pristine, with current values
      },
      error: err => {
        console.error('Update failed:', err);
        this.toastr.error('Failed to update profile');
      }
    });
  }

  onSubmit() {
    this.updateMember();
  }

  goToProfile() {
    if (this.member?.userName) {
      this.router.navigate(['/profile', this.member.userName]);
    }
  }
}