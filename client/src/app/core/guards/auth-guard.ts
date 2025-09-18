import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account-service';
import { map, of } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
 //canActivate est une fonction qui décide si une route peut s’activer
  
  const accountService = inject(AccountService); //lit ton état d’authentification (ici via un signal currentUser())
  const router = inject(Router);

  if(accountService.currentUser()){
    return of(true);
  }
  else{
    return accountService.getAuthState().pipe(
      map (auth => {
        if(auth.isAuthenticated){
          return true;
        } else {
          router.navigate(['/account/login'], {queryParams: {returnUrl: state.url}});
          return false;
        }
      })
    )
    
  }

  //router.navigate([...], { queryParams: { returnUrl: state.url } })
  /* Annule la nav en cours et lance une nouvelle navigation vers /account/login.
  Ajoute le paramètre de requête ?returnUrl=... contenant l’URL que l’utilisateur 
  voulait ouvrir (state.url, ex. /checkout?step=2).
  But : après un login réussi, ton LoginComponent lit returnUrl et fait router.navigateByUrl(returnUrl) pour revenir exactement là où l’utilisateur voulait aller. */
};
