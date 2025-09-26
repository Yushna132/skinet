import { Component, inject, Input } from '@angular/core';
import { CartService } from '../../../core/services/cart-service';
import { CurrencyPipe } from '@angular/common';
import { ConfirmationToken } from '@stripe/stripe-js';
import { AddressPipe } from "../../../shared/pipes/address-pipe";
import { PaymentCardPipe } from "../../../shared/pipes/payment-card-pipe";

@Component({
  selector: 'app-checkout-review',
  imports: [
    CurrencyPipe,
    AddressPipe,
    PaymentCardPipe
],
  templateUrl: './checkout-review.html',
  styleUrl: './checkout-review.scss'
})
export class CheckoutReview {
  cartService = inject(CartService);
  //Faire passer le jeton de confirmation (créé dans le parent CheckoutComponent)
  //vers le composant enfant CheckoutReviewComponent.
  @Input() confirmationToken?: ConfirmationToken;

}
