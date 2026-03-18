import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiService } from './api.service';
import { Category } from '../models/category.model';
import { CreateCategoryDto } from '../models/category-create.model';

@Injectable({ providedIn: 'root' })

export class CategoryService {

  private http = inject(HttpClient);

    constructor(private api: ApiService) {}


      /**
   * Get all categories
   */
  getAll(){
    return this.api.get<Category[]>('categories');
  }
  list(params?: any): Observable<any> {
    return this.http.get(`${environment.apiBase}/categories`, { params });
  }

  getById(id: string | number): Observable<any> {
    return this.http.get(`${environment.apiBase}/categories/${id}`);
  }

  create(payload: CreateCategoryDto): Observable<Category> {
    return this.api.post<Category>('categories', payload);
  }

  update(id: string | number, payload: any): Observable<any> {
    return this.http.put(`${environment.apiBase}/categories/${id}`, payload);
  }

  delete(id: string | number): Observable<any> {
    return this.http.delete(`${environment.apiBase}/categories/${id}`);
  }
}
