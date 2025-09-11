import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { User } from '../_models/user';
import { map } from 'rxjs';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';
import { getDecodedToken } from '../_helpers/jwt-helper';
import { Router } from '@angular/router';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private http = inject(HttpClient);
  private authService = inject(AuthService);
  private presenceService = inject(PresenceService);
  private router: Router = inject(Router);
  baseUrl = environment.apiUrl;
  currentUser = signal<User | null>(null);

  constructor(router: Router) {
    this.router = router;
  }
  
  login(model: any) {

    const headers = new HttpHeaders({ 'skip-spinner': 'true' });

    return this.http.post<User>(this.baseUrl + 'account/login', model, {headers}).pipe(
      map(user => {
        if (user && user.token) {
          const decoded = getDecodedToken(user.token);
          user.roles = Array.isArray(decoded.role) ? decoded.role : [decoded.role];

          this.setCurrentUser(user);
          this.authService.login(user.token, user);
        }
        return user; // <-- RETURN user
      })
    );
  }

  register(model: any) {

  const headers = new HttpHeaders({ 'skip-spinner': 'true' });

  return this.http.post<User>(this.baseUrl + 'account/register', model, {headers}).pipe(
    map(user => {
      if (user && user.token) {
        const decoded = getDecodedToken(user.token);
        user.roles = decoded.role instanceof Array ? decoded.role : [decoded.role];

        this.setCurrentUser(user);
        this.authService.login(user.token, user);
      }
      return user;
    })
  );
}

setCurrentUser(user: User) {
  localStorage.setItem('user', JSON.stringify(user));
  this.currentUser.set(user);
  this.presenceService.createHubConnection(user);


}

  logout() {
  localStorage.removeItem('user');
  localStorage.removeItem('token');  // make sure token is removed too
  this.currentUser.set(null);        // clear signal
  this.presenceService.stopHubConnection();
  this.router.navigate(['/']); // redirect to Home
}

}