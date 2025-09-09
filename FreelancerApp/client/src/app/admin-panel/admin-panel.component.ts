import { Component, inject, OnInit } from '@angular/core';
import { UserAdminDTO } from '../_DTOs/userAdminDTO';
import { CommonModule, DatePipe } from '@angular/common';
import { PaginatedResultAdmin } from '../_models/paginationAdmin';
import { MembersService } from '../_services/members.service';

@Component({
  standalone: true,
  selector: 'app-admin-panel',
  imports: [DatePipe, CommonModule],
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.css']
})
export class AdminPanelComponent implements OnInit {

  users: UserAdminDTO[] = [];
  filteredUsers: UserAdminDTO[] = [];
  selectedRole: string = 'Freelancer';
  pagination: { currentPage: number; totalPages: number; itemsPerPage: number; totalItems: number } = {
    currentPage: 1,
    totalPages: 1,
    itemsPerPage: 11,
    totalItems: 0
  };

  membersService = inject(MembersService);

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(page: number = 1) {
  this.membersService.getAdminUsers(page, this.pagination.itemsPerPage).subscribe(res => {
    console.log('Admin API response:', res);
    // If res is a plain array:
    this.users = res as UserAdminDTO[];
    this.pagination = { currentPage: 1, totalPages: 1, itemsPerPage: 10, totalItems: this.users.length };
    console.log('Users array:', this.users);
    this.filterUsers();
  });
}

  filterUsers() {
    if (!this.users) return;
    this.filteredUsers = this.users.filter(u =>
      u.roles.some(r => r.toLowerCase() === this.selectedRole.toLowerCase())
    );
  }

  selectRole(role: string) {
    this.selectedRole = role;
    this.filterUsers();
  }

  loadPage(page: number) {
    if (page < 1 || page > this.pagination.totalPages) return;
    this.loadUsers(page);
  }

  enableUser(user: UserAdminDTO) {
    this.membersService.enableUser(user.id).subscribe(() => {
      user.isAccountDisabled = false;
    });
  }

  disableUser(user: UserAdminDTO) {
    this.membersService.disableUser(user.id).subscribe(() => {
      user.isAccountDisabled = true;
    });
  }

  pagesArray(): number[] {
    return Array(this.pagination.totalPages).fill(0).map((x, i) => i + 1);
  }
}