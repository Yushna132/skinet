import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // HttpRequest est immuable → on le clone
  const clonedRequest = req.clone({
    withCredentials: true // ajoute toujours les cookies
  })
  return next(clonedRequest);
};
