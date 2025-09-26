import { Pipe, PipeTransform } from '@angular/core';
import { ConfirmationToken } from '@stripe/stripe-js';

@Pipe({
  name: 'paymentCard',
})
export class PaymentCardPipe implements PipeTransform {
  transform(value?: ConfirmationToken['payment_method_preview'], ...args: unknown[]): unknown {
    const card = value?.card;
    if (card) {
      return `${card.brand.toUpperCase()} **** **** **** ${card.last4} – Exp: ${card.exp_month}/${
        card.exp_year
      }`;
    } else {
      return 'Card Invalid';
    }
  }
}
