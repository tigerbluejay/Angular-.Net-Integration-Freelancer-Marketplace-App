import { NgIf } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AccountService } from '../../_services/account.service';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [RouterLink, NgIf],
  templateUrl: './not-found.component.html',
  styleUrl: './not-found.component.css'
})
export class NotFoundComponent {
  accountService = inject(AccountService);

  redirectLink(): string {
    const user = this.accountService.currentUser();
    return user ? '/profile' : '/';
  }
}
