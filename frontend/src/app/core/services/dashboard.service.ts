import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private http = inject(HttpClient);

  summary(): Observable<any> {
    return this.http.get(`${environment.apiBase}/dashboard/summary`);
  }

  charts(): Observable<any> {
    return this.http.get(`${environment.apiBase}/dashboard/charts`);
  }
}
