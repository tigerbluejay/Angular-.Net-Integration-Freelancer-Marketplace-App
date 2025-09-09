import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders, HttpParams, HttpXhrBackend } from '@angular/common/http';
import { Member } from '../_models/member';
import { map, of, tap } from 'rxjs';
import { ProjectDTO } from '../_DTOs/projectDTO';
import { PortfolioItemDTO } from '../_DTOs/portfolioItemDTO';
import { UserAdminDTO } from '../_DTOs/userAdminDTO';
import { PaginatedResultAdmin } from '../_models/paginationAdmin';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  // Signal to hold cached members
  members = signal<Member[]>([]);

  // Get all members and cache them
  getMembers() {
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      tap(fetchedMembers => this.members.set(fetchedMembers))
    );
  }

  // Get single member from cache if available
  getMember(username: string) {
    const member = this.members().find(x => x.userName === username);
    if (member) return of(member);

    const headers = new HttpHeaders({ 'skip-spinner': 'true' });

    return this.http.get<Member>(this.baseUrl + 'users/' + username, { headers });
  }

  // PUT update for currently logged-in member
  updateMember(member: Member) {

    return this.http.put(this.baseUrl + 'users/update-profile', member).pipe(
      tap(() => {
        this.members.update(members =>
          members.map(m => m.userName === member.userName ? member : m)
        );
      })
    );
  }

  // Fetch client projects paginated
  getClientProjects(username: string, pageNumber: number = 1, pageSize: number = 6) {
    let params = new HttpParams()
      .set('PageNumber', pageNumber.toString())
      .set('PageSize', pageSize.toString());

    return this.http.get<ProjectDTO[]>(`${this.baseUrl}users/${username}/projects`, { observe: 'response', params });
  }

  // Fetch freelancer portfolio paginated
  getFreelancerPortfolio(username: string, pageNumber: number = 1, pageSize: number = 6) {
    let params = new HttpParams()
      .set('PageNumber', pageNumber.toString())
      .set('PageSize', pageSize.toString());

    return this.http.get<PortfolioItemDTO[]>(`${this.baseUrl}users/${username}/portfolio`, { observe: 'response', params });
  }

  getAdminUsers(pageNumber: number = 1, pageSize: number = 10) {
  let params = new HttpParams()
    .set('pageNumber', pageNumber.toString())
    .set('pageSize', pageSize.toString());

  return this.http.get<UserAdminDTO[]>(this.baseUrl + 'users/admin', { params });
}

  // Disable a user
  disableUser(id: number) {
    return this.http.patch(this.baseUrl + `users/${id}/disable`, {});
  }

  // Enable a user
  enableUser(id: number) {
    return this.http.patch(this.baseUrl + `users/${id}/enable`, {});
  }
}
