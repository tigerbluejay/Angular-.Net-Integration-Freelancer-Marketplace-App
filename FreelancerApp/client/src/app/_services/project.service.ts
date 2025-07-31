import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';


@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  baseUrl = environment.apiUrl; // "https://localhost:5001/api/"

  constructor(private http: HttpClient) {}

  deleteProject(id: number) {
    return this.http.delete(this.baseUrl + 'project/' + id);
  }

}