import { Component, inject } from '@angular/core';
import {MatIcon} from '@angular/material/icon';
import {MatButton} from '@angular/material/button';
import {MatBadge} from '@angular/material/badge';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { Busy } from '../../core/service/busy';
import {MatProgressBar} from '@angular/material/progress-bar';
import { CartService } from '../../core/services/cart-service';
import { AccountService } from '../../core/services/account-service';
import {MatMenu, MatMenuItem, MatMenuModule, MatMenuTrigger} from '@angular/material/menu';
import { MatDivider } from '@angular/material/divider';


@Component({
  selector: 'app-header',
  imports: [
    MatIcon,
    MatButton,
    MatBadge,
    RouterLink,
    RouterLinkActive,
    MatProgressBar,
    MatMenuTrigger,
    MatMenu,
    MatDivider,
    MatMenuItem
],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class Header {
  busyService = inject(Busy);
  cartService = inject(CartService);
  accoutService = inject(AccountService);
  private router = inject(Router); 

  logout(){
    this.accoutService.logout().subscribe({
      next: () => {
        // vider l’état local
        this.accoutService.currentUser.set(null);
        // rediriger (ex. page d’accueil)
        this.router.navigateByUrl('/');
      }
    })
  }

}
