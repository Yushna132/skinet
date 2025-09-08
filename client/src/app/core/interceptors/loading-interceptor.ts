import { HttpInterceptorFn } from '@angular/common/http';
import { delay, finalize } from 'rxjs';
import { Busy } from '../service/busy';
import { inject } from '@angular/core';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {

    const busyService = inject(Busy);
    busyService.busy();

  return next(req).pipe(
    delay(500),
    finalize(() => busyService.idle())
  );
  
};
