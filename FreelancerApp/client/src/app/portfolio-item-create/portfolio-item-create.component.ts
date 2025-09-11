import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PortfolioItem } from '../_models/portfolio-item';
import { Member } from '../_models/member';
import { PortfolioItemService } from '../_services/portfolio-item.service';
import { AccountService } from '../_services/account.service';
import { MembersService } from '../_services/members.service';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { PhotoService } from '../_services/photo.service';

@Component({
  standalone: true,
  selector: 'app-portfolio-item-create',
  templateUrl: './portfolio-item-create.component.html',
  imports: [CommonModule, FormsModule]
})
export class PortfolioItemCreateComponent implements OnInit {
  member: Member | undefined;
  portfolioItem: PortfolioItem | null = null;
  isEditMode = false;

  selectedPhotoFile?: File;
  photoPreviewUrl: string | null | undefined;

  @ViewChild('editForm') editForm?: NgForm;

  constructor(
    private activeRouter: ActivatedRoute,
    private router: Router,
    private membersService: MembersService,
    private portfolioItemService: PortfolioItemService,
    private accountService: AccountService,
    private photoService: PhotoService,
    private toastr: ToastrService     // <--- inject ToastrService
  ) { }

  ngOnInit(): void {
    const id = this.activeRouter.snapshot.paramMap.get('id');
    this.isEditMode = !!id;

    this.loadMemberAndPortfolioItem(Number(id));
  }

  loadMemberAndPortfolioItem(id: number) {
    const user = this.accountService.currentUser();
    if (!user) return;

    this.membersService.getMember(user.username).subscribe({
      next: member => {
        this.member = member;

        if (this.isEditMode) {
          this.portfolioItem = member.portfolioItems.find(p => p.id === id) ?? null;

          if (!this.portfolioItem) {
            this.toastr.error('Portfolio item not found.');
          } else {
            this.photoPreviewUrl = this.portfolioItem.photoUrl || undefined;
          }
        } else {
          this.portfolioItem = {
            id: 0,
            photoUrl: '',
            title: '',
            description: '',
            created: new Date().toISOString()
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

    if (!this.portfolioItem) return;

    this.photoService.deletePortfolioPhoto(this.portfolioItem.id).subscribe({
      next: () => {
        this.toastr.success('Photo deleted successfully');
        this.photoPreviewUrl = undefined;
        this.selectedPhotoFile = undefined;
        this.portfolioItem!.photoUrl = '';

        // Optionally mark form dirty or trigger UI update here
      },
      error: err => {
        this.toastr.error('Failed to delete photo');
        console.error(err);
      }
    });
  }

  onSubmit() {
    if (!this.portfolioItem) return;

    if (this.isEditMode) {
      // Editing existing portfolio item
      if (this.selectedPhotoFile) {
        // Upload photo first
        this.photoService.uploadPortfolioPhoto(this.portfolioItem.id, this.selectedPhotoFile).subscribe({
          next: photo => {
            this.toastr.success('Photo uploaded successfully');
            if (photo.url) {
              this.portfolioItem!.photoUrl = photo.url;
            }
            // Then update portfolio item with photoUrl
            this.savePortfolioItem();
          },
          error: err => {
            console.error('Photo upload failed:', err);
            this.toastr.error('Failed to upload photo');
          }
        });
      } else {
        // Just update portfolio item without photo upload
        this.savePortfolioItem();
      }
    } else {
      // Creating new portfolio item
      this.portfolioItemService.createPortfolioItem(this.portfolioItem).subscribe({
        next: createdItem => {
          this.toastr.success('Portfolio item created successfully');

          // Reset form so guard doesn’t trigger
          this.editForm?.reset(createdItem);

          if (this.selectedPhotoFile) {
            // Upload photo now with the new portfolioItemId
            this.photoService.uploadPortfolioPhoto(createdItem.id, this.selectedPhotoFile).subscribe({
              next: photo => {
                if (photo.url) {
                  createdItem.photoUrl = photo.url;
                }
                // Update the portfolio item again with the photoUrl
                this.portfolioItemService.updatePortfolioItem(createdItem.id, createdItem).subscribe({
                  next: () => this.goToProfile(),
                  error: err => {
                    this.toastr.error('Failed to update portfolio item with photo.');
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
          this.toastr.error('Failed to create portfolio item.');
          console.error(err);
        }
      });
    }
  }


  private savePortfolioItem() {
    if (!this.portfolioItem) return;

    if (this.isEditMode) {
      this.portfolioItemService.updatePortfolioItem(this.portfolioItem.id, this.portfolioItem).subscribe({
        next: () => {
          this.toastr.success('Portfolio item updated successfully.');
          // Reset form so guard doesn’t trigger
          this.editForm?.reset(this.portfolioItem);
          this.goToProfile();
        },
        error: err => {
          this.toastr.error('Failed to update portfolio item.');
          console.error(err);
        }
      });
    } else {
      this.portfolioItemService.createPortfolioItem(this.portfolioItem).subscribe({
        next: () => {
          this.toastr.success('Portfolio item created successfully.');
          // Reset form so guard doesn’t trigger
          this.editForm?.reset(this.portfolioItem);
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
}