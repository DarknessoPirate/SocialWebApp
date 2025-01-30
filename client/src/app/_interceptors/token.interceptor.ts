import { HttpInterceptorFn } from '@angular/common/http';
import { AccountService } from '../_services/account.service';
import { inject } from '@angular/core';

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
   const accountService = inject(AccountService)

   if(accountService.currentUser()){
      // intercept the request and overwrite it with added header, request is immutable so we need to clone it and rewrite the reference
      req = req.clone({
         setHeaders: {
            Authorization: `Bearer ${accountService.currentUser()?.token}`
         }
      })
   }

   // pass the new request 
   return next(req);
};
