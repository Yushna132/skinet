import { Component, Input } from '@angular/core';
import { Product } from '../../../shared/models/product';
import { MatCard, MatCardActions, MatCardContent } from '@angular/material/card';
import { CurrencyPipe } from '@angular/common';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';

@Component({
  selector: 'app-product-item-component',
  imports: [
    MatCard, 
    MatCardContent, 
    MatCardActions, 
    MatButton, 
    MatIcon, 
    CurrencyPipe],
  templateUrl: './product-item-component.html',
  styleUrl: './product-item-component.scss',
})
export class ProductItemComponent {
  //@Input est utilisé pour recuperer les product du ShopComponent
  // ? car la donnée peut arriver après la création du composant
  @Input() product?: Product;
}
