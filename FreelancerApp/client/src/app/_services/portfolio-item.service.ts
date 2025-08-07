import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { PortfolioItem } from '../_models/portfolio-item';


@Injectable({
  providedIn: 'root'
})
export class PortfolioItemService {
  baseUrl = environment.apiUrl; // "https://localhost:5001/api/"

  constructor(private http: HttpClient) { }

  deletePortfolioItem(id: number) {
  
    return this.http.delete(this.baseUrl + 'portfolioitem/' + id);
  }

  createPortfolioItem(item: PortfolioItem) {

    return this.http.post<PortfolioItem>(this.baseUrl + 'portfolioitem', item);
  }

  updatePortfolioItem(id: number, item: PortfolioItem) {

    return this.http.put(this.baseUrl + 'portfolioitem/' + id, item);
  }

}