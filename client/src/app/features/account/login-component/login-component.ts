import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatCard } from '@angular/material/card';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { AccountService } from '../../../core/services/account-service';
import { ActivatedRoute, Router } from '@angular/router';
import { authGuard } from '../../../core/guards/auth-guard';

@Component({
  selector: 'app-login-component',
  imports: [
    ReactiveFormsModule,
    MatCard,
    MatFormField,
    MatInput,
    MatLabel,
    MatButton
  ],
  templateUrl: './login-component.html',
  styleUrl: './login-component.scss'
})
export class LoginComponent {
  private fb = inject(FormBuilder); //pour créer le formulaire
  private accountService = inject(AccountService); //pour communiquer avec l’API.
  private router = inject(Router); //pour rediriger après connexion.
  private activatedRoute = inject(ActivatedRoute);
  returnUrl = '/shop'

  constructor(){
    const url= this.activatedRoute.snapshot.queryParams['returnUrl'];
    if(url) this.returnUrl = url;
    
  }

  loginForm = this.fb.group({
    email: [''],
    password: ['']
  });

  onSubmit(){
    this.accountService.login(this.loginForm.value).subscribe({
      next : () =>{
        this.accountService.getUserInfo().subscribe(); // récupère les infos utilisateur
      // this.router.navigateByUrl('/shop'); // redirige vers la boutique
        this.router.navigateByUrl(this.returnUrl);
      }
    })
  }


}
