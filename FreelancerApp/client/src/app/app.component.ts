import { Component, computed, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NgIf, NgFor } from '@angular/common';
import { AccountService } from './_services/account.service';
import { FreelancernavComponent } from './nav/freelancernav/freelancernav.component';
import { ClientnavComponent } from './nav/clientnav/clientnav.component';
import { PublicnavComponent } from './nav/publicnav/publicnav.component';
import { NgxSpinnerComponent } from 'ngx-spinner';
import { TimeagoModule } from 'ngx-timeago';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    NgIf,
    FreelancernavComponent,
    ClientnavComponent,
    PublicnavComponent,
    NgxSpinnerComponent,
    TimeagoModule
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'] // <-- plural
})
export class AppComponent {
  private accountService = inject(AccountService);

  // computed signal for user role
  readonly userRole = computed(() => this.accountService.currentUser()?.roles?.[0] ?? null);

  // computed signal for logged-in status
  readonly isLoggedIn = computed(() => !!this.accountService.currentUser());

  constructor() {
    this.setCurrentUserFromLocalStorage();
  }

  private setCurrentUserFromLocalStorage() {
    const userString = localStorage.getItem('user');
    if (!userString) return;
    const user = JSON.parse(userString);
    this.accountService.currentUser.set(user);
  }
}