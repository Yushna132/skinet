import { Component, inject, OnInit, signal } from '@angular/core';
import { Header } from './layout/header/header';
import { Product } from './shared/models/product';
import { ShopService } from './core/services/shop-service';
import { ShopComponent } from "./features/shop-component/shop-component";
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Header, ShopComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent{
  // protected readonly title = signal('client');
  title = 'Skinet';
}
