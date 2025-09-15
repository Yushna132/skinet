import { Component, inject } from '@angular/core';
import {MatIcon} from '@angular/material/icon';
import {MatButton} from '@angular/material/button';
import {MatBadge} from '@angular/material/badge';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { Busy } from '../../core/service/busy';
import {MatProgressBar} from '@angular/material/progress-bar';
import { CartService } from '../../core/services/cart-service';


@Component({
  selector: 'app-header',
  imports: [
    MatIcon,
    MatButton,
    MatBadge,
    RouterLink,
    RouterLinkActive,
    MatProgressBar
],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class Header {
  busyService = inject(Busy);
  cartService = inject(CartService);

}
