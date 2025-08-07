import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Project } from '../_models/project';


@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  baseUrl = environment.apiUrl; // "https://localhost:5001/api/"

  constructor(private http: HttpClient) {}

  deleteProject(id: number) {
    
    return this.http.delete(this.baseUrl + 'project/' + id);
  }

    createProject(project: Project) {
    
      return this.http.post<Project>(this.baseUrl + 'project', project);
    }
  
    updateProject(id: number, project: Project) {
            
      return this.http.put(this.baseUrl + 'project/' + id, project);
    }
  

}