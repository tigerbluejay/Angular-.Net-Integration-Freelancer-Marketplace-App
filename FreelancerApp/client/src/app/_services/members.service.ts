import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders, HttpXhrBackend } from '@angular/common/http';
import { Member } from '../_models/member';
import { of, tap } from 'rxjs';

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

    return this.http.get<Member>(this.baseUrl + 'users/' + username, {headers});
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


}
