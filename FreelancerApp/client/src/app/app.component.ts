import { NgFor, NgIf } from '@angular/common';

import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AccountService } from './_services/account.service';
import { HomeComponent } from "./home/home.component";
import { FreelancernavComponent } from './nav/freelancernav/freelancernav.component';
import { AuthService } from './_services/auth.service';
import { PublicnavComponent } from './nav/publicnav/publicnav.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NgFor, FreelancernavComponent, NgIf, HomeComponent, PublicnavComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit{
  private accountService = inject(AccountService);
  private authService = inject(AuthService);
  title = 'FreelancerApp';
  isLoggedIn = false;
  userRole: string | null = null;

  ngOnInit(): void {
      this.authService.isLoggedIn$.subscribe(status => {
      this.isLoggedIn = status;
    });
    this.setCurrentUser();
  }

  setCurrentUser() {
    const userString = localStorage.getItem('user');
    if (!userString) return;
    const user = JSON.parse(userString);
    this.accountService.currentUser.set(user);
  }

}  