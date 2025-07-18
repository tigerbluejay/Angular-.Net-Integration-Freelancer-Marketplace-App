import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private isLoggedInSubject = new BehaviorSubject<boolean>(false);

  // Public observables your components will subscribe to
  isLoggedIn$ = this.isLoggedInSubject.asObservable();

  constructor() {
    const token = localStorage.getItem('token');

    if (token) {
      this.isLoggedInSubject.next(true);
    }
  }

  login(token: string) {
    localStorage.setItem('token', token);
    this.isLoggedInSubject.next(true);
  }

  logout() {
    localStorage.removeItem('token');
    this.isLoggedInSubject.next(false);
  }
}