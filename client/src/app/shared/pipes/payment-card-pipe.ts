import { Pipe, PipeTransform } from '@angular/core';
import { ConfirmationToken } from '@stripe/stripe-js';
import { PaymentSummary } from '../models/order';

@Pipe({
  name: 'paymentCard',
})
export class PaymentCardPipe implements PipeTransform {
  transform(value?: ConfirmationToken['payment_method_preview']| PaymentSummary, ...args: unknown[]): unknown {
    /* const card = value?.card;
    if (card) {
      return `${card.brand.toUpperCase()} **** **** **** ${card.last4} – Exp: ${card.exp_month}/${
        card.exp_year
      }`;
    } else {
      return 'Card Invalid';
    } */

    const card = value;
    if (value && 'card' in value) {
      const{brand,last4,exp_month,exp_year} = (value as ConfirmationToken['payment_method_preview'])?.card!;
      return `${brand.toUpperCase()} **** **** **** ${last4} – Exp: ${exp_month}/${exp_year}`;
    } 
    else if(value && 'cardBrand' in value) 
    {
      const{cardBrand,last4,expMonth,expYear} = value as PaymentSummary
      return `${cardBrand.toUpperCase()} **** **** **** ${last4} – Exp: ${expMonth}/${expYear}`;
    }
    else {
      return 'Card Invalid';
    }
  }
}
