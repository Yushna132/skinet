import { Component, inject } from '@angular/core';
import { CartService } from '../../core/services/cart-service';
import { CartItemComponent } from "./cart-item-component/cart-item-component";
import { OrderSummary } from "../../shared/components/order-summary/order-summary";

@Component({
  selector: 'app-cart',
  imports: [CartItemComponent, OrderSummary],
  templateUrl: './cart.html',
  styleUrl: './cart.scss'
})
export class Cart {
  cartService = inject(CartService);

}
