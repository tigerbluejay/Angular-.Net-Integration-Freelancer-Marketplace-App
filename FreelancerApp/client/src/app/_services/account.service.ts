import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { User } from '../_models/user';
import { map } from 'rxjs';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';
import { getDecodedToken } from '../_helpers/jwt-helper';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private http = inject(HttpClient);
  private authService = inject(AuthService);
  baseUrl = environment.apiUrl;
  currentUser = signal<User | null>(null);

  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map(user => {
        if (user && user.token) {
          const decoded = getDecodedToken(user.token);
          user.roles = Array.isArray(decoded.role) ? decoded.role : [decoded.role];

          localStorage.setItem('user', JSON.stringify(user));
          this.currentUser.set(user);
          this.authService.login(user.token);
        }
        return user; // <-- RETURN user
      })
    );
  }

  register(model: any) {
  return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
    map(user => {
      if (user && user.token) {
        const decoded = getDecodedToken(user.token);
        user.roles = decoded.role instanceof Array ? decoded.role : [decoded.role];

        localStorage.setItem('user', JSON.stringify(user));
        this.currentUser.set(user);
        this.authService.login(user.token);
      }
      return user;
    })
  );
}

setCurrentUser(user: User) {
  this.currentUser.set(user);
}

  logout() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
    this.authService.logout();
  }

}