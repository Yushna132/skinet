import { Component, inject, OnInit } from '@angular/core';
import { ShopService } from '../../../core/services/shop-service';
import { ActivatedRoute } from '@angular/router';
import { Product } from '../../../shared/models/product';
import { CurrencyPipe } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatFormField } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import {MatFormFieldModule} from '@angular/material/form-field';
import { MatDivider } from "@angular/material/divider";
@Component({
  selector: 'app-product-details',
  imports: [
    CurrencyPipe,
    MatButton,
    MatIcon,
    MatFormField,
    MatInput,
    MatFormFieldModule,
    MatDivider
],
  templateUrl: './product-details.html',
  styleUrl: './product-details.scss'
})
export class ProductDetails implements OnInit{
  
  private shopService = inject(ShopService);
  private activatedRoute = inject(ActivatedRoute);
  product? :  Product;
  ngOnInit(): void {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if(!id) return;
    this.shopService.getProduct(+id).subscribe({
      next: product => this.product = product,
      error: error => console.log(error)
    })
  }
    
  

}
