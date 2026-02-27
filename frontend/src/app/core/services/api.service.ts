import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';

/**
 * ApiService
 *
 * Centralized HTTP service.
 * - Prepends base URL
 * - Unwraps ApiResponse<T>
 * - Keeps feature services clean
 */
@Injectable({
  providedIn: 'root'
})
export class ApiService {

  private baseUrl = environment.apiBase;

  constructor(private http: HttpClient) {}

  /**
   * Generic GET method
   */
  get<T>(endpoint: string): Observable<T> {
    return this.http
      .get<ApiResponse<T>>(`${this.baseUrl}/${endpoint}`)
      .pipe(map(response => response.data));
  }

  /**
   * Generic POST method
   */
  post<T>(endpoint: string, body: any): Observable<T> {
    return this.http
      .post<ApiResponse<T>>(`${this.baseUrl}/${endpoint}`, body)
      .pipe(map(response => response.data));
  }

  /**
   * Generic PUT method
   */
  put<T>(endpoint: string, body: any): Observable<T> {
    return this.http
      .put<ApiResponse<T>>(`${this.baseUrl}/${endpoint}`, body)
      .pipe(map(response => response.data));
  }

  /**
   * Generic DELETE method
   */
  delete<T>(endpoint: string): Observable<T> {
    return this.http
      .delete<ApiResponse<T>>(`${this.baseUrl}/${endpoint}`)
      .pipe(map(response => response.data));
  }
}