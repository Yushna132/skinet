import { Component, inject, OnDestroy } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { SignalrService } from '../../../core/services/signalr-service';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { PaymentCardPipe } from '../../../shared/pipes/payment-card-pipe';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AddressPipe } from '../../../shared/pipes/address-pipe';
import { OrderService } from '../../../core/services/order-service';

@Component({
  selector: 'app-checkout-success-component',
  imports: [
    MatButton,
    RouterLink,
    MatProgressSpinnerModule,
    DatePipe,
    AddressPipe,
    CurrencyPipe,
    PaymentCardPipe
  ],
  templateUrl: './checkout-success-component.html',
  styleUrl: './checkout-success-component.scss'
})
export class CheckoutSuccessComponent implements OnDestroy {
  /* Sur la page Checkout Success, on:
    affiche un spinner pendant qu’on attend le webhook Stripe → API,
    écoute SignalR ; dès que l’API envoie "OrderCompleteNotification",
    on hydrate l’UI avec la commande reçue (DTO) et on propose le lien “voir la commande”. */
  signalrService  = inject(SignalrService);
  private orderService = inject(OrderService)

   ngOnDestroy(): void {
    this.orderService.orderComplete = false;
    this.signalrService.orderSignal.set(null); //nettoyage order dans UI
  }

}
