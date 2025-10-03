import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Order, OrderToCreate } from '../../shared/models/order';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  orderComplete = false; //flag utilié dans checkout et checkout-success pour pas avoir accés à /checkout-success en tapant l’URL

  createOrder(orderToCreate: OrderToCreate){
    return this.http.post<Order>(this.baseUrl+'orders', orderToCreate);
  }

  getOrdersForUser(){
    return this.http.get<Order[]>(this.baseUrl+'orders');
  }

  getOrderDetailed(id: number){
    return this.http.get<Order>(this.baseUrl+'orders/'+id);
  }

  
}
