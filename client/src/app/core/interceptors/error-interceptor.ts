import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { Snackbar } from '../services/snackbar';

//On creer une fonction de type HttpInterceptorFn
export const errorInterceptor: HttpInterceptorFn = (req, next) => {

  //modification avant de l'envoyer dans l'API
  const router = inject(Router);
  const snackbar = inject(Snackbar)
  
  //.pipe nous permet de manipuler les reponses(Observables) sans que on subscribes au reponses 
  return next(req).pipe(
    catchError((err: HttpErrorResponse) =>{
    
      //code 1
    
      if (err.status === 400) {
        if (err.error.errors) {
          const modalStateErrors = [];
          for (const key in err.error.errors) {
            if (err.error.errors[key]) {
              modalStateErrors.push(err.error.errors[key])
            }
          }
          throw modalStateErrors.flat();
        } else {
          snackbar.error(err.error.title || err.error)
        }
      }

      
      if(err.status === 401){
        snackbar.error(err.error.title || err.error)
      }
      
      if(err.status === 404){
        router.navigateByUrl('/not-found');
      }
      
      if(err.status === 500){
        const navigationExtras: NavigationExtras = {state: {error: err.error}}
        router.navigateByUrl('/server-error');
      }
      return throwError(() => err)
    })

  

  );
  //modification aprés l'avoir reçu de l'API
  //on peut le faire pour ratrapper les erreurs
};
