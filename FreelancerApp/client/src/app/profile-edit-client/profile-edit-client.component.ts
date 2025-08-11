import { Component, HostListener, inject, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Member } from '../_models/member';
import { AccountService } from '../_services/account.service';
import { MembersService } from '../_services/members.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { PhotoService } from '../_services/photo.service';

@Component({
  selector: 'app-profile-edit-client',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './profile-edit-client.component.html',
  styleUrl: './profile-edit-client.component.css'
})
export class ProfileEditClientComponent {
  @ViewChild('editForm') editForm?: NgForm;

  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.editForm?.dirty) {
      $event.returnValue = true;
    }
  }

  member?: Member;
  selectedPhotoFile?: File;
  photoPreviewUrl: string | null | undefined;

  private accountService = inject(AccountService);
  private membersService = inject(MembersService);
  private photoService = inject(PhotoService);
  private toastr = inject(ToastrService);
  router = inject(Router);

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    this.photoPreviewUrl = this.member?.photoUrl || undefined;

    const user = this.accountService.currentUser();
    if (!user) return;

    this.membersService.getMember(user.username).subscribe({
      next: member => {
        this.member = member;
        // Set preview to current profile picture if it exists
        if (member.photoUrl) {
          this.photoPreviewUrl = member.photoUrl;
        }
      },
      error: err => this.toastr.error('Failed to load profile')
    });
  }


  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedPhotoFile = input.files[0];

      // Create preview
      const reader = new FileReader();
      reader.onload = () => {
        this.photoPreviewUrl = reader.result as string;
      };
      reader.readAsDataURL(this.selectedPhotoFile);
    }

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
    if (!this.member) return;
    if (this.selectedPhotoFile) {
      this.photoService.uploadUserPhoto(this.selectedPhotoFile).subscribe({
        next: photo => {
          this.toastr.success('Photo uploaded successfully');
          // Update both the model and the preview with the server URL
          if (photo.url) {
            this.member!.photoUrl = photo.url;
            this.photoPreviewUrl = photo.url;
            // 🔹 Update AccountService currentUser so navbar updates
            const user = this.accountService.currentUser();
            if (user) {
              this.accountService.setCurrentUser({ ...user, photoUrl: photo.url });
            }
          }
          // Continue with member update
          this.updateMember();
        },
        error: err => {
          console.error('Photo upload failed:', err);
          this.toastr.error('Failed to upload photo');
        }
      });

    } else {
      this.updateMember();
    }
  }

  onDeletePhoto() {
    if (!confirm('Are you sure you want to delete your profile photo?')) return;
    this.photoService.deleteUserPhoto().subscribe({
      next: (message: string) => {
        console.log('Delete success:', message);
        this.toastr.success('Photo deleted successfully');
        if (this.member) this.member.photoUrl = undefined;
        this.photoPreviewUrl = undefined;
        this.selectedPhotoFile = undefined;
        // 🔹 Update AccountService currentUser so navbar updates
        const user = this.accountService.currentUser();
        if (user) {
          this.accountService.setCurrentUser({ ...user, photoUrl: '' });
        }
        this.editForm?.form.markAsDirty(); // optional, to mark form dirty
      },
      error: err => {
        console.error('Failed to delete photo', err);
        this.toastr.error('Failed to delete photo');
      }
    });
  }

  goToProfile() {
    if (this.member?.userName) {
      this.router.navigate(['/profile', this.member.userName]);
    }
  }
}