import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { NotificationService } from '../../shared/services/notification.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {

  const router = inject(Router);
  const notification = inject(NotificationService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const isAuthRequest = req.url.includes('/auth/login') || req.url.includes('/auth/register');
      const message = getErrorMessage(error);

      if (error.status === 401) {
        if (typeof window !== 'undefined' && window?.localStorage) {
          window.localStorage.removeItem('token');
        }

        if (isAuthRequest) {
          notification.error(message || 'Invalid email or password.');
        } else {
          notification.warning('Your session has expired. Please sign in again.');
          router.navigate(['/login']);
        }
      }

      if (error.status === 403) {
        notification.error(message || 'Access denied.');
      }

      if (error.status === 400) {
        notification.error(message || 'Please check the information you entered.');
      }

      if (error.status === 500) {
        notification.error(message || 'Server error occurred. Please try again later.');
      }

      if (error.status === 0) {
        notification.error('Network error. Check your connection and try again.');
      }

      return throwError(() => error);
    })
  );
};

function getErrorMessage(error: HttpErrorResponse): string | null {
  const payload = error.error;

  if (typeof payload === 'string' && payload.trim()) {
    return payload;
  }

  if (payload?.message && typeof payload.message === 'string') {
    return payload.message;
  }

  if (payload?.error && typeof payload.error === 'string') {
    return payload.error;
  }

  if (Array.isArray(payload)) {
    const firstMessage = payload
      .map(item => item?.description || item?.code)
      .find((item): item is string => typeof item === 'string' && item.trim().length > 0);

    return firstMessage ?? null;
  }

  return null;
}
