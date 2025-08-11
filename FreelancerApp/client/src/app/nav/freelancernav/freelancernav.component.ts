import { Component, computed, effect, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIf, TitleCasePipe } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { AccountService } from '../../_services/account.service';
import { AuthService } from '../../_services/auth.service';
import { User } from '../../_models/user';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-freelancernav',
  standalone: true,
  imports: [FormsModule, NgIf, BsDropdownModule, TitleCasePipe, RouterLink, RouterLinkActive],
  templateUrl: './freelancernav.component.html',
  styleUrl: './freelancernav.component.css'
})

export class FreelancernavComponent {
  accountService = inject(AccountService);
  authService = inject(AuthService);
  private router = inject(Router);

  username = computed(() => this.accountService.currentUser()?.username ?? null);
  photoTimestamp = Date.now();

  constructor() {
    // Create an effect that runs whenever currentUser changes
    effect(() => {
      // Access the signal to register dependency
      const user = this.accountService.currentUser();
      // Update timestamp to bust cache on user change
      this.photoTimestamp = Date.now();
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
