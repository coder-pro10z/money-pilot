import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(private api: ApiService) {}
  private http = inject(HttpClient);
   

  login(credentials: { email: string; password: string }): Observable<any> {
    return this.http.post(`${environment.apiBase}/auth/login`, credentials).pipe(
      tap((res: any) => {
        if (res?.token && typeof window !== 'undefined' && window?.localStorage) {
          window.localStorage.setItem('token', res.token);
        }
      })
    );
  }

  
  register(data:any){
  return this.api.post('auth/register', data);
}



  logout(): void {
    if (typeof window !== 'undefined' && window?.localStorage) {
      window.localStorage.removeItem('token');
    }
  }

  getToken(): string | null {
    if (typeof window === 'undefined' || !window?.localStorage) return null;
    return window.localStorage.getItem('token');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}
