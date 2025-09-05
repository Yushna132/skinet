import { Routes } from '@angular/router';
import { Home } from './features/home/home';
import { ShopComponent } from './features/shop-component/shop-component';
import { ProductDetails } from './features/shop/product-details/product-details';

export const routes: Routes = [
    {path:'', component: Home},
    {path:'shop', component: ShopComponent},
    {path:'shop/:id', component: ProductDetails},
    {path:'**', redirectTo:'', pathMatch:'full'},
];
