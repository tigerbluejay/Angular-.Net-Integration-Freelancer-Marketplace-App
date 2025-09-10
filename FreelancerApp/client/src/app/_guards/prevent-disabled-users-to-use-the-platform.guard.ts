// account.guard.ts
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { ModalService } from '../_services/modal.service';
import { AccountService } from '../_services/account.service';

export const accountGuard: CanActivateFn = () => {
  const modalService = inject(ModalService);
  const accountService = inject(AccountService);
  const router = inject(Router);

  const userJson = localStorage.getItem('user');
  const user = userJson ? JSON.parse(userJson) : null;

  if (user?.isAccountDisabled) {
    console.log('AccountGuard: user is disabled, showing modal');
    modalService.open(
      'disabledAccountModal',
      'Account Disabled',
      'Your account has been disabled. Please contact admin@example.com to re-enable your account.',
      () => {
        accountService.logout();
        router.navigate(['/']); // redirect to home/login
      }
    );

    return false; // block navigation
  }

  return true; // allow navigation
};