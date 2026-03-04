import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {

  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {

      if (error.status === 401) {
        console.warn('Unauthorized. Redirecting to login.');
        localStorage.removeItem('token');
        router.navigate(['/login']);
      }

      if (error.status === 403) {
        alert('Access denied.');
      }

      if (error.status === 400) {
        const message = error.error?.message || 'Invalid request.';
        alert(message);
      }

      if (error.status === 500) {
        alert('Server error occurred. Please try again later.');
      }

      if (error.status === 0) {
        alert('Network error. Check your connection.');
      }

      return throwError(() => error);
    })
  );
};