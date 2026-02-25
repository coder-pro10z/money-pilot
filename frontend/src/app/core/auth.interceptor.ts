import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  if (typeof window === 'undefined' || !window?.localStorage) {
    return next(req);
  }

  const token = window.localStorage.getItem('token');
  const authReq = token ? req.clone({ headers: req.headers.set('Authorization', `Bearer ${token}`) }) : req;
  return next(authReq);
};
