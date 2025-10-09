import { inject, Injectable } from '@angular/core';
import {ConfirmationToken, loadStripe, Stripe, StripeAddressElement, StripeAddressElementOptions, StripeElements, StripePaymentElement} from '@stripe/stripe-js';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { CartService } from './cart-service';
import { Cart } from '../../shared/models/cart';
import { firstValueFrom, map } from 'rxjs';
import { AccountService } from './account-service';


@Injectable({
  providedIn: 'root'
})
export class StripeService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient)
  private cartService = inject(CartService);
  private accountService = inject(AccountService); // Pour obtenir l'address de l'utilisateur

  // Une seule instance Stripe pour toute l’app
  private stripePromise : Promise<Stripe | null>; 
  private elements?: StripeElements;
  private addressElement?: StripeAddressElement;
  private paymentElement?: StripePaymentElement;
  

  constructor() {
    this.stripePromise = loadStripe(environment.stripePublicKey);
  }

  /** Récupère l’instance Stripe (Promise<Stripe|null>) */
  getStripeInstance() {
    return this.stripePromise;
  }

   /**
   * Initialise Stripe Elements UNE SEULE FOIS avec le clientSecret du panier
   * Doit être appelé avant de monter les éléments (adresse/carte).
   */
  async initializeElements(){
    if(!this.elements){
      const stripe = await this.getStripeInstance();
      if(stripe){
        //Quand la personne a rempli tous les informations
        const cart = await firstValueFrom(this.createOrUpdatePaymentIntent());
        this.elements = stripe.elements(
            {clientSecret: cart.clientSecret, appearance: {labels: 'floating'}})
      }
      else{
        throw new Error('Stripe has not been loaded');
      }
    }
    return this.elements;
  }

 //création (unique) de l’Address Element (mode "shipping")
 async createAddressElement(){
    //Ça évite de créer plusieurs fois le même composant Stripe dans ta page checkout.
    //sinon Stripe plante, car chaque élément doit être unique)
    if(!this.addressElement){
      const elements = await this.initializeElements(); 
      if(elements)
        {
           // 1) Lire l’utilisateur courant depuis un signal/observable exposé par AccountService
          const user = this.accountService.currentUser();
          // 2) Préparer les valeurs par défaut pour Stripe
          let defaultValues: StripeAddressElementOptions['defaultValues']= {};
          if(user){
            defaultValues.name = user.firstName+ ' '+ user.lastName;
          }
          if(user?.address){
            defaultValues.address = {
              line1: user.address.line1,
              line2: user.address.line2,
              city: user.address.city,
              state: user.address.state,
              country: user.address.country,
              postal_code: user.address.postalCode
            }
          }
          
          const options : StripeAddressElementOptions = {
            mode: 'shipping', 
            defaultValues // 3) Créer l’Address Element (une seule instance)
          };
          this.addressElement = elements.create('address', options); //Là Stripe génère son widget d’adresse, prêt à être monté dans ton HTML.
        } 
        else
        {
          throw new Error('Elements instance has not been loaded');
        }
    }
    return this.addressElement;
 }

 


 //création (unique) de payment Element 
 async createPaymentElement(){
   if(!this.paymentElement){
      const elements = await this.initializeElements();
      if(elements){
        this.paymentElement = elements.create('payment')
      }
      else{
        throw new Error('Elements instance has not been initialized');
      }
    }
   return this.paymentElement;
}

async createConfirmationToken(){
    const stripe = await this.getStripeInstance();
    const elements = await this.initializeElements();
    const result = await elements.submit();
    if(result.error) throw new Error(result.error.message);
    if(stripe){
      return await stripe.createConfirmationToken({elements});
    }  else{
      throw new Error('Stripe not available');
    }
 }

//valider Elements, récupérer client_secret de l’Intent, puis confirmer avec le confirmation_token
async confirmPayment(confirmation_token: ConfirmationToken){
    const stripe = await this.getStripeInstance();
    const elements = await this.initializeElements();
    const result = await elements.submit();
    if(result.error) throw new Error(result.error.message); //Toujours valider les éléments (wallets, champs, etc.)

    const clientSecret = this.cartService.cart()?.clientSecret;
    // Récupérer le client_secret (stocké côté app après création/maj de PaymentIntent)
    if(stripe && clientSecret){
      // Confirmer le paiement avec le token
      return await stripe.confirmPayment({
        clientSecret: clientSecret,
        confirmParams : {
          confirmation_token: confirmation_token.id // pas d'URL de retour ici : on gère la navigation dans le composant
        }, 
        redirect: 'if_required' // évite redirection automatique; on gère depuis Angular
      })
    }
    else{
      throw new Error('Unable to load stripe');
    }
}

  /**
   * Crée/MàJ la PaymentIntent via l'API .NET, synchronise le panier
   * Retourne le panier à jour (avec paymentIntentId + clientSecret)
   */
  createOrUpdatePaymentIntent(){
    const cart = this.cartService.cart();
    
    if(!cart) throw new Error('Problem with cart');

    return this.http.post<Cart>(this.baseUrl + 'payment/' + cart.id, {}).pipe( 
      
      map( async cart => {
       // this.cartService.cart.set(cart); // maj locale du signal (évite un aller-retour)
        await firstValueFrom()this.cartService.setCart(cart));
        return cart;
      })
    )
  }

  disposeElement(){
    this.elements = undefined;
    this.addressElement = undefined; //si tu veux forcer une recréation plus tard
    this.paymentElement = undefined
  }

   
  
}
