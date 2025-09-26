import { CurrencyPipe, Location } from '@angular/common';
import { Component, inject } from '@angular/core';
import { CartService } from '../../../core/services/cart-service';
import { RouterLink } from '@angular/router';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';


@Component({
  selector: 'app-order-summary',
  imports: [
    CurrencyPipe,
    RouterLink,
    MatFormField,
    MatLabel,
    MatInput
],
  templateUrl: './order-summary.html',
  styleUrl: './order-summary.scss'
})
export class OrderSummary {
  cartService = inject(CartService);
  location = inject(Location);

}
