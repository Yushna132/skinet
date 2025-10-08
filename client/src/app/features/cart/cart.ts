import { Component, inject } from '@angular/core';
import { CartService } from '../../core/services/cart-service';
import { CartItemComponent } from "./cart-item-component/cart-item-component";
import { OrderSummary } from "../../shared/components/order-summary/order-summary";
import { EmptyStateComponent } from "../../shared/components/empty-state-component/empty-state-component";
import { Router } from '@angular/router';

@Component({
  selector: 'app-cart',
  imports: [CartItemComponent, OrderSummary, EmptyStateComponent],
  templateUrl: './cart.html',
  styleUrl: './cart.scss'
})
export class Cart {
  private router = inject(Router);
  cartService = inject(CartService);

  onAction(){
    this.router.navigateByUrl('/shop');
  }

}
