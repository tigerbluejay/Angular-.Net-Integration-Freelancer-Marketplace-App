import { Component, inject } from '@angular/core';
import { AccountService } from '../../_services/account.service';
import { AuthService } from '../../_services/auth.service';
import { User } from '../../_models/user';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-publicnav',
  standalone: true,
  imports: [FormsModule, NgIf, BsDropdownModule, RouterLink, RouterLinkActive],
  templateUrl: './publicnav.component.html',
  styleUrl: './publicnav.component.css'
})
export class PublicnavComponent {
  accountService = inject(AccountService);
  authService = inject(AuthService);
  private router = inject(Router);
  private toastr = inject(ToastrService);

  model: any = {};

login() {
  this.accountService.login(this.model).subscribe({
    next: (user: User) => {
      // Check user role and navigate accordingly
      if (user.roles[0] === 'Freelancer') {
        this.router.navigate(['/browse-projects']); // replace with your actual route
      } else if (user.roles[0] === 'Client') {
        this.router.navigate(['/profile', user.username]);
      } else if (user.roles[0] === 'Admin') {
        this.router.navigate(['/admin-panel']);
      } else {
        // default fallback
        this.router.navigate(['/']);
      }
    },
    error: error => this.toastr.error(error.error)
  });
}

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }


}
