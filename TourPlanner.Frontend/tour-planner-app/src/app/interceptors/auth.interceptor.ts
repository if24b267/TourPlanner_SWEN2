import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  if (token) {
    req = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      // Abgelaufener/ungueltiger Token: ohne das hier wuerde die App weiter die
      // Hauptansicht zeigen und jeder Request wuerde still mit 401 fehlschlagen,
      // ohne dass der Nutzer einen Hinweis bekommt.
      if (err.status === 401 && authService.getToken()) {
        authService.handleUnauthorized();
      }
      return throwError(() => err);
    })
  );
};