import { HttpInterceptorFn } from '@angular/common/http';
import { delay, finalize, identity } from 'rxjs';
import { Busy } from '../service/busy';
import { inject } from '@angular/core';
import { environment } from '../../../environments/environment';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {

    const busyService = inject(Busy);
    busyService.busy();

  return next(req).pipe(
    //identity = équivalent de “ne rien faire(null)
    (environment.production ? identity : delay(500)),
    finalize(() => busyService.idle())
  );
  
};
