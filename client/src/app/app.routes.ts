import { Routes } from '@angular/router';
import { Home } from './features/home/home';
import { ShopComponent } from './features/shop-component/shop-component';
import { ProductDetails } from './features/shop/product-details/product-details';
import { TestError } from './features/test-error/test-error';
import { NotFound } from './shared/components/not-found/not-found';
import { ServerError } from './shared/components/server-error/server-error';

export const routes: Routes = [
    {path:'', component: Home},
    {path:'shop', component: ShopComponent},
    {path:'shop/:id', component: ProductDetails},
    {path:'test-error', component: TestError},
    {path:'not-found', component: NotFound},
    {path:'server-error', component: ServerError},
    {path:'**', redirectTo:'not-found', pathMatch:'full'},
];
