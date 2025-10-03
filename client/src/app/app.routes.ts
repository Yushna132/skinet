import { Routes } from '@angular/router';
import { Home } from './features/home/home';
import { ShopComponent } from './features/shop-component/shop-component';
import { ProductDetails } from './features/shop/product-details/product-details';
import { TestError } from './features/test-error/test-error';
import { NotFound } from './shared/components/not-found/not-found';
import { ServerError } from './shared/components/server-error/server-error';
import { Cart } from './features/cart/cart';
import { Checkout } from './features/checkout/checkout';
import { LoginComponent } from './features/account/login-component/login-component';
import { ResgisterComponent } from './features/account/resgister-component/resgister-component';
import { authGuard } from './core/guards/auth-guard';
import { emptyCartGuard } from './core/guards/empty-cart-guard';
import { CheckoutSuccessComponent } from './features/checkout/checkout-success-component/checkout-success-component';
import { OrderComponent } from './features/orders/order-component';
import { OrderDetailedComponent } from './features/orders/order-detailed-component/order-detailed-component';
import { orderCompleteGuard } from './core/guards/order-complete-guard';


export const routes: Routes = [
    {path:'', component: Home},
    {path:'shop', component: ShopComponent},
    {path:'shop/:id', component: ProductDetails},
    {path:'cart', component: Cart},
    {path:'checkout', component: Checkout, canActivate: [authGuard, emptyCartGuard]},
    {path:'checkout/success', component: CheckoutSuccessComponent, canActivate: [authGuard, orderCompleteGuard]},
    {path:'orders', component: OrderComponent, canActivate: [authGuard]},
    {path:'orders/:id', component: OrderDetailedComponent, canActivate: [authGuard]},
    {path:'account/login', component: LoginComponent},
    {path:'account/register', component: ResgisterComponent},
    {path:'test-error', component: TestError},
    {path:'not-found', component: NotFound},
    {path:'server-error', component: ServerError},
    {path:'**', redirectTo:'not-found', pathMatch:'full'},
];
