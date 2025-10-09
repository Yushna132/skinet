import { Component, inject, OnInit, output } from '@angular/core';
import { Checkout } from '../../../core/services/checkout';
import {MatRadioModule} from '@angular/material/radio';
import { CurrencyPipe } from '@angular/common';
import { CartService } from '../../../core/services/cart-service';
import { DeliveryMethod } from '../../../shared/models/deliveryMethod';

@Component({
  selector: 'app-checkout-delivery-component',
  imports: [
    MatRadioModule,
    CurrencyPipe
  ],
  templateUrl: './checkout-delivery-component.html',
  styleUrl: './checkout-delivery-component.scss'
})
export class CheckoutDeliveryComponent implements OnInit {
  
  checkoutService = inject(Checkout);
  cartService = inject(CartService); //on a besoin du cart service afin de modifier le delivery free(shipping) dans la carte
  deliveryComplete = output<boolean>(); //transmettre du composant enfant vers le parent (Angular @Output) -> Pour savoir si le delivery est completé
  
  ngOnInit(): void {
    this.checkoutService.getDeliveryMethods().subscribe(
      {
        next: methods => { //methods => réprésente tous les méthodes
          //si on a deja un delivery method que l'utilisateur a choisi et de le mettre en mode selection
          if(this.cartService.cart()?.deliveryMethodId){
            const method = methods.find(x => x.id === this.cartService.cart()?.deliveryMethodId); //On cherche la method de livraison a travers par le id
            if(method){
              this.cartService.selectedDelivery.set(method); //On met à jour le signal Angular selectedDelivery avec cet objet complet.
              this.deliveryComplete.emit(true);
            }
          }
        }
      }
    );
  }

  async updateDeliveryMethod(method: DeliveryMethod){
    this.cartService.selectedDelivery.set(method); //On met à jour le signal Angular selectedDelivery avec cet objet complet.
    //maintenant on met à jour la carte
    const cart = this.cartService.cart();
    if(cart){
      cart.deliveryMethodId = method.id;
      await firstValueFrom(this.cartService.setCart(cart)); //MAJ du carte dans le backend
      this.deliveryComplete.emit(true);
    }
  }

  

}
