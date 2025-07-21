import { Component, inject } from '@angular/core';
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
  private toastr = inject(ToastrService);

  model: any = {};

  login() {
    this.accountService.login(this.model).subscribe({
      next: (response: User) => {
        const token = response.token;
        if (token) {
          this.authService.login(token);
          console.log('Logged in:', response.username);
          this.router.navigate(['/profile']);
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
