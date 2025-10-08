import { Component, inject, input, output } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { Busy } from '../../../core/service/busy';

@Component({
  selector: 'app-empty-state-component',
  imports: [
    MatIcon,
    MatButton,
    RouterLink
  ],
  templateUrl: './empty-state-component.html',
  styleUrl: './empty-state-component.scss'
})
export class EmptyStateComponent {
  busyService = inject(Busy)
  message = input.required<string>();
  icon = input.required<string>(); // ex. 'filter_alt_off' (Angular Material)
  actionText = input.required<string>();  // ex. 'Réinitialiser les filtres'
  action = output<void>();

  onAction(){
    this.action.emit();
  }

}
