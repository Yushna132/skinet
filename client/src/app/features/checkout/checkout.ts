import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { OrderSummary } from "../../shared/components/order-summary/order-summary";
import {MatStepper, MatStepperModule} from '@angular/material/stepper';
import { Router, RouterLink } from '@angular/router';
import { MatButton } from '@angular/material/button';
import { StripeService } from '../../core/services/stripe-service';
import { ConfirmationToken, StripeAddressElement, StripeAddressElementChangeEvent, StripePaymentElement, StripePaymentElementChangeEvent } from '@stripe/stripe-js';
import { Snackbar } from '../../core/services/snackbar';
import {MatCheckboxChange, MatCheckboxModule} from '@angular/material/checkbox';
import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { Address } from '../../shared/models/user';
import { AccountService } from '../../core/services/account-service';
import { firstValueFrom } from 'rxjs';
import { CheckoutDeliveryComponent } from "./checkout-delivery-component/checkout-delivery-component";
import { CheckoutReview } from "./checkout-review/checkout-review";
import { CartService } from '../../core/services/cart-service';
import { CurrencyPipe, JsonPipe } from '@angular/common';
import { JsonpInterceptor } from '@angular/common/http';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';



@Component({
  selector: 'app-checkout',
  imports: [
    OrderSummary,
    MatStepperModule,
    RouterLink,
    MatButton,
    MatCheckboxModule,
    CheckoutDeliveryComponent,
    CheckoutReview,
    CurrencyPipe, 
    JsonPipe,
    MatProgressSpinnerModule
],
  templateUrl: './checkout.html',
  styleUrl: './checkout.scss'
})
export class Checkout implements OnInit, OnDestroy {
  
  
  private stripeService = inject(StripeService);
  private snackBar = inject(Snackbar);
  private router = inject(Router)
  private accountService = inject(AccountService);
  addressElement?: StripeAddressElement;
  paymentElement?: StripePaymentElement;
  cartService = inject(CartService);
  saveAddress = false;
  /* signal : permet de réagir facilement dans le template.
     Chaque clé est un booléen (par défaut false). */
  completionStatus = signal<{ address: boolean, card: boolean, delivery: boolean }>(
    { address: false, card: false, delivery: false });
  confirmationToken?: ConfirmationToken;
  loading = false;

  /* constructor(){
    this.handleAddressChange = this.handleAddressChange.bind(this);
  } */

  async ngOnInit(){
    try {
      this.addressElement = await this.stripeService.createAddressElement();
      this.addressElement.mount('#address-element'); //Stripe injecte automatiquement le formulaire d’adresse complet dans ce <div>
      this.addressElement.on('change', this.handleAddressChange)

      this.paymentElement = await this.stripeService.createPaymentElement();
      this.paymentElement.mount('#payment-element');
      this.paymentElement.on('change', this.handlePaymentChange);

    } catch (error : any) {
      this.snackBar.error(error.message);
    }
  }

  handleAddressChange = (event: StripeAddressElementChangeEvent) => {
    this.completionStatus.update(state => {
      state.address = event.complete;
      return state;
    })
  }

  handlePaymentChange = (event: StripePaymentElementChangeEvent) => {
    this.completionStatus.update(state => {
      state.card = event.complete;
      return state;
    })
  }

  handleDeliveryChange(event : boolean){
    this.completionStatus.update(state => {
      state.delivery = event;
      return state;
    })
  }

  async getConfirmationToken(){
    try {
      if(Object.values(this.completionStatus()).every(status => status === true)){
        const result = await this.stripeService.createConfirmationToken();
        if(result.error) throw new Error(result.error.message);
        this.confirmationToken = result.confirmationToken;
        console.log(this.confirmationToken);
      }
    } catch (error : any) {
      this.snackBar.error(error.message);
      
    }
  }

 async onStepChange(event: StepperSelectionEvent ){
    //Verifier si on a passer à la page suivant
    if(event.selectedIndex === 1){
      //si le checkbox est checked i.e true
      if(this.saveAddress){
        const address = await this.getAddressFromStripeAddress();
        /* Est équivalent à ça
          if (address) {
          await firstValueFrom(this.accountService.updateAddress(address));
          } */
        address && firstValueFrom(this.accountService.updateAddress(address));
       
      }
    }

    if(event.selectedIndex === 2){
      await firstValueFrom(this.stripeService.createOrUpdatePaymentIntent()); //firstValueFrom(obs) → convertit un Observable en Promise, mais uniquement en prenant la première valeur émise par l’Observable.
    }

    if(event.selectedIndex === 3){
      await this.getConfirmationToken();
    }

  }

  async confirmPayment(stepper:MatStepper){
    this.loading = true;
    try {
      if(this.confirmationToken){
        const result = await this.stripeService.confirmPayment(this.confirmationToken);
        if(result.error){
          // Erreur Stripe (ex: carte refusée)(par le  back)
          throw new Error(result.error.message);
        }
        else
        {
          // Succès : nettoyer l’état de checkout 
          this.cartService.deleteCart();
          this.cartService.selectedDelivery.set(null);
          this.router.navigateByUrl('checkout/success'); // Redirection
        }
      }
    } catch (error : any) {
      this.snackBar.error(error.message || 'Something went wrong') ;
      stepper.previous(); // Revenir à l’étape Paiement pour ressaisir / modifier la carte
    } finally {
      this.loading = false;
    }
  }
  private async getAddressFromStripeAddress() : Promise<Address | null> {
    const result = await this.addressElement?.getValue();
    const address = result?.value.address;
    if(address){
      return{
        line1: address.line1,
        line2: address.line2 || undefined,
        city: address.city,
        country: address.country,
        state: address.state,
        postalCode: address.postal_code
      }
    }
    else return null;
  }

  onSaveAddressCheckboxChange(event : MatCheckboxChange){
    this.saveAddress = event.checked;
  }

  ngOnDestroy(): void {
    this.stripeService.disposeElement();
  }
}
