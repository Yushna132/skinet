import { HttpErrorResponse } from '@angular/common/http';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { MatCard } from "@angular/material/card";

@Component({
  selector: 'app-server-error',
  imports: 
  [
    MatCard
  ],
  templateUrl: './server-error.html',
  styleUrl: './server-error.scss'
})
export class ServerError {
  error?: any;

  constructor(private router: Router) {
    const navigation = this.router.getCurrentNavigation();
    this.error = navigation?.extras.state?.['error'];
  }


}
