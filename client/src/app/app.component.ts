import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Header } from './layout/header/header';
import { HttpClient } from '@angular/common/http';
import { Product } from './shared/models/product';
import { Pagination } from './shared/models/pagination';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Header],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
  // protected readonly title = signal('client');
  baseUrl = 'https://localhost:5001/api/';
  private http = inject(HttpClient);
  title = 'Skinet';
  //declarer un products de type any
  products: Product[] = [];

  ngOnInit(): void {
    //ici <any> fait référence a products qui est de type any
    this.http.get<Pagination<Product>>(this.baseUrl + 'products').subscribe({
      next: (response) => (this.products = response.data),
      error: (error) => console.log(error),
      complete: () => console.log('complete'),
    });
  }
}
