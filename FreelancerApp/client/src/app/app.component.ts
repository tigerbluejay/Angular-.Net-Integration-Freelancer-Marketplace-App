import { NgFor, NgIf } from '@angular/common';

import { Component, computed, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AccountService } from './_services/account.service';
import { HomeComponent } from "./home/home.component";
import { FreelancernavComponent } from './nav/freelancernav/freelancernav.component';
import { AuthService } from './_services/auth.service';
import { PublicnavComponent } from './nav/publicnav/publicnav.component';
import { ClientnavComponent } from './nav/clientnav/clientnav.component';
import { map } from 'rxjs';
import { NgxSpinnerComponent } from 'ngx-spinner';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NgFor, FreelancernavComponent, NgIf, HomeComponent, 
    PublicnavComponent, ClientnavComponent, NgxSpinnerComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit{
  private accountService = inject(AccountService);
  private authService = inject(AuthService);
  title = 'FreelancerApp';
  isLoggedIn = false;
  readonly userRole = computed(() => this.accountService.currentUser()?.roles?.[0] ?? null);

  ngOnInit(): void {
    this.logoutOnInit();
    this.setCurrentUser();
    this.authService.isLoggedIn$.subscribe(status => {
    this.isLoggedIn = status;
    });
  }

  setCurrentUser() {
    const userString = localStorage.getItem('user');
    if (!userString) return;
    const user = JSON.parse(userString);
    this.accountService.currentUser.set(user);
  }

  logoutOnInit() {
  this.accountService.logout();
}

}  