import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Project } from '../_models/project';
import { PaginatedResult, Pagination } from '../_models/pagination';
import { map, Observable } from 'rxjs';
import { ProjectParams } from '../_models/projectParams';
import { ProjectBrowseDTO } from '../_DTOs/projectBrowseDTO';
import { ProjectActiveDTO } from '../_DTOs/projectActiveDTO';
import { ProposalwithProjectAssignCombinedDTO } from '../_DTOs/proposalwithProjectAssignCombinedDTO_tmp';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  baseUrl = environment.apiUrl; // e.g. "https://localhost:5001/api/"

  constructor(private http: HttpClient) { }

  // 🔹 GET projects with filtering & pagination
  getProjects(params: ProjectParams) {
    let httpParams = new HttpParams();

    if (params.pageNumber != null) httpParams = httpParams.set('PageNumber', params.pageNumber);
    if (params.pageSize != null) httpParams = httpParams.set('PageSize', params.pageSize);

    httpParams = httpParams.set('IncludeAssigned', String(!!params.includeAssigned));
    httpParams = httpParams.set('MatchAllSkills', String(!!params.matchAllSkills));

    (params.skillNames ?? []).forEach(s => {
      if (s && s.trim().length) httpParams = httpParams.append('SkillNames', s.trim());
    });

    return this.http
      .get<ProjectBrowseDTO[]>(this.baseUrl + 'project', {
        params: httpParams,
        observe: 'response'
      })
      .pipe(
        map(res => {
          const paginated = new PaginatedResult<ProjectBrowseDTO[]>();
          paginated.result = res.body ?? [];

          const raw =
            res.headers.get('Pagination') ??
            res.headers.get('X-Pagination');

          if (raw) {
            paginated.pagination = JSON.parse(raw) as Pagination;
          } else {
            // fallback minimal pagination if header missing
            paginated.pagination = {
              currentPage: params.pageNumber ?? 1,
              itemsPerPage: params.pageSize ?? paginated.result.length,
              totalItems: paginated.result.length,
              totalPages: 1
            };
          }

          return paginated;
        })
      );
  }

  deleteProject(id: number) {
    return this.http.delete(this.baseUrl + 'project/' + id);
  }

  createProject(project: Project) {
    return this.http.post<Project>(this.baseUrl + 'project', project);
  }

  updateProject(id: number, project: Project) {
    return this.http.put(this.baseUrl + 'project/' + id, project);
  }

  getProjectById(id: number): Observable<ProjectBrowseDTO> {
    return this.http.get<ProjectBrowseDTO>(`${this.baseUrl}project/${id}`);
  }

  patchProposalWithProjectAssign(
    dto: ProposalwithProjectAssignCombinedDTO
  ): Observable<ProposalwithProjectAssignCombinedDTO> {
    return this.http.patch<ProposalwithProjectAssignCombinedDTO>(
      `${this.baseUrl}project/assign-proposals-with-projects`,
      dto // <-- you need to pass the body of the PATCH here
    );
  }

    getProjectsByClientId(
    clientId: number,
    pageNumber: number,
    pageSize: number
  ): Observable<ProjectActiveDTO[]> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<ProjectActiveDTO[]>(
      `${this.baseUrl}project/projects-by-client-id/${clientId}`,
      { params }
    );
  }

  getProjectsByFreelancerId(
    freelancerId: number,
    pageNumber: number,
    pageSize: number
  ): Observable<ProjectActiveDTO[]> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<ProjectActiveDTO[]>(
      `${this.baseUrl}project/projects-by-freelancer-id/${freelancerId}`,
      { params }
    );
  }
}