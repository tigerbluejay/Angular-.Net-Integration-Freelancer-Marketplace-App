import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProposalCreateDTO } from '../_DTOs/proposalCreateDTO';
import { Proposal } from '../_models/proposal';
import { ProposalWithProjectCombinedDTO } from '../_DTOs/proposalWithProjectCombinedDTO';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProposalService {

  baseUrl = environment.apiUrl + 'proposals/';

  constructor(private http: HttpClient) {}

  createProposal(dto: ProposalCreateDTO): Observable<Proposal> {
    const formData = new FormData();

    formData.append('title', dto.title);
    if (dto.description) {
      formData.append('description', dto.description);
    }
    formData.append('bid', dto.bid.toString());
    formData.append('projectId', dto.projectId.toString());
    formData.append('freelancerUserId', dto.freelancerUserId.toString());
    formData.append('clientUserId', dto.clientUserId.toString());

    if (dto.photoFile) {
      formData.append('photoFile', dto.photoFile, dto.photoFile.name);
    }

    return this.http.post<Proposal>(this.baseUrl, formData);
  }

  getProposalById(id: number): Observable<Proposal> {
    return this.http.get<Proposal>(this.baseUrl + id);
  }

  // ✅ New endpoint: get proposals + associated projects for a freelancer
  getProposalsWithProjects(freelancerId: number): Observable<ProposalWithProjectCombinedDTO[]> {
    return this.http.get<ProposalWithProjectCombinedDTO[]>(
      `${this.baseUrl}proposals-with-projects/${freelancerId}`
    );
  }
}