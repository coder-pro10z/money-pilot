import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private http = inject(HttpClient);

  list(params?: any): Observable<any> {
    return this.http.get(`${environment.apiBase}/categories`, { params });
  }

  get(id: string | number): Observable<any> {
    return this.http.get(`${environment.apiBase}/categories/${id}`);
  }

  create(payload: any): Observable<any> {
    return this.http.post(`${environment.apiBase}/categories`, payload);
  }

  update(id: string | number, payload: any): Observable<any> {
    return this.http.put(`${environment.apiBase}/categories/${id}`, payload);
  }

  delete(id: string | number): Observable<any> {
    return this.http.delete(`${environment.apiBase}/categories/${id}`);
  }
}
