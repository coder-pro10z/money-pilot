import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DashboardSummary } from '../models/dashboard.model';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  // private http = inject(HttpClient);
  constructor(private api: ApiService){}
 /**
   * Fetch dashboard summary
   */
  getSummary(): Observable<DashboardSummary> {
    return this.api.get<DashboardSummary>(`dashboard/summary`);
  }

  charts(): Observable<any> {
    return this.api.get<DashboardSummary>(`dashboard/charts`);
  }
}
