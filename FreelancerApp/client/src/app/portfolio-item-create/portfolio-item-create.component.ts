import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PortfolioItem } from '../_models/portfolio-item';
import { Member } from '../_models/member';
import { PortfolioItemService } from '../_services/portfolio-item.service';
import { AccountService } from '../_services/account.service';
import { MembersService } from '../_services/members.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

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

  constructor(
    private activeRouter: ActivatedRoute,
    private router: Router,
    private membersService: MembersService,
    private portfolioItemService: PortfolioItemService,
    private accountService: AccountService,
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
          }
        } else {
          this.portfolioItem = {
            id: 0,
            photoUrl: '',
            title: '',
            description: '',
            created: new Date().toISOString()
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
    if (!this.portfolioItem) return;

    if (this.isEditMode) {
      this.portfolioItemService.updatePortfolioItem(this.portfolioItem.id, this.portfolioItem).subscribe({
        next: () => {
          this.toastr.success('Portfolio item updated successfully.');
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