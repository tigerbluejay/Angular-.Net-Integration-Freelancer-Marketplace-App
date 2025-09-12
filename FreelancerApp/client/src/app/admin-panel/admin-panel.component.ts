import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { UserAdminDTO } from '../_DTOs/userAdminDTO';
import { CommonModule, DatePipe } from '@angular/common';
import { MembersService } from '../_services/members.service';
import { PresenceService } from '../_services/presence.service';

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
  pagination = { currentPage: 1, totalPages: 1, itemsPerPage: 11, totalItems: 0 };

  membersService = inject(MembersService);
  private presenceService = inject(PresenceService);

  // Computed signal for reactivity
  isUserOnline = computed(() => {
    return (username: string) => this.presenceService.onlineUsers().includes(username);
  });

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(page: number = 1) {
    this.membersService.getAdminUsers(page, this.pagination.itemsPerPage).subscribe(res => {
      this.users = res as UserAdminDTO[];
      this.pagination = { ...this.pagination, totalItems: this.users.length };
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
    this.membersService.enableUser(user.id).subscribe(() => user.isAccountDisabled = false);
  }

  disableUser(user: UserAdminDTO) {
    this.membersService.disableUser(user.id).subscribe(() => user.isAccountDisabled = true);
  }

  pagesArray(): number[] {
    return Array(this.pagination.totalPages).fill(0).map((x, i) => i + 1);
  }

  isOnline(user: UserAdminDTO): boolean {
    // Normalize both: lowercase and trim spaces
    const onlineUsers = this.presenceService.onlineUsers().map(u => u.toLowerCase().trim());
    const knownAs = (user.knownAs || '').toLowerCase().trim();

    const isOnline = onlineUsers.includes(knownAs);

    // Optional: log for debugging
    console.log(`Checking online status for '${user.knownAs}' -> ${isOnline}`, 'Online users:', onlineUsers);

    return isOnline;
  }
}