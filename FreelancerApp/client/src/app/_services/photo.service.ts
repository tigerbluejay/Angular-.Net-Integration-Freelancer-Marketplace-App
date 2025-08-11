import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Photo } from '../_models/photo';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PhotoService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl + 'photos/';

  // ---------- UPLOAD ----------
  uploadUserPhoto(file: File): Observable<Photo> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<Photo>(this.baseUrl + 'user', formData);
  }

  uploadProjectPhoto(projectId: number, file: File): Observable<Photo> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<Photo>(this.baseUrl + `project/${projectId}`, formData);
  }

  uploadPortfolioPhoto(portfolioItemId: number, file: File): Observable<Photo> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<Photo>(this.baseUrl + `portfolio/${portfolioItemId}`, formData);
  }

  // ---------- DELETE ----------
  deleteUserPhoto(): Observable<string> {
  return this.http.delete(this.baseUrl + 'user', { responseType: 'text' });
}

  deleteProjectPhoto(projectId: number) {
    return this.http.delete(this.baseUrl + `project/${projectId}`, { responseType: 'text' });
  }

  deletePortfolioPhoto(portfolioItemId: number) {
    return this.http.delete(this.baseUrl + `portfolio/${portfolioItemId}`, { responseType: 'text' });
  }
}