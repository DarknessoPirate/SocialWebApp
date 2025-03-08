import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../_services/account.service';

export const guestGuard: CanActivateFn = (route, state) => {
   const accountService = inject(AccountService);
   const router = inject(Router);

   if (!accountService.currentUser()) {
      return true;
   } else {
      router.navigate(['/home']); // Redirect logged-in users to home
      return false;
   }
};