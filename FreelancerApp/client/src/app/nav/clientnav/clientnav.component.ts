import { Component, computed, inject } from '@angular/core';
import { AuthService } from '../../_services/auth.service';
import { AccountService } from '../../_services/account.service';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgIf, TitleCasePipe } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';

@Component({
  selector: 'app-clientnav',
  standalone: true,
  imports: [FormsModule, NgIf, BsDropdownModule, TitleCasePipe, RouterLink, RouterLinkActive],
  templateUrl: './clientnav.component.html',
  styleUrl: './clientnav.component.css'
})
export class ClientnavComponent {

  accountService = inject(AccountService);
  authService = inject(AuthService);
  private router = inject(Router);
  username = computed(() => this.accountService.currentUser()?.username ?? null);


  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

}
