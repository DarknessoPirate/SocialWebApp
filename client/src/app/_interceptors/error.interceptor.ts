import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
   const router = inject(Router);
   const toastr = inject(ToastrService);


   return next(req).pipe(
      catchError(error => {
         if (error) {
            switch (error.status) {
               case 400:
                  if (error.error && Array.isArray(error.error)) {
                     // if the error is an array of objects 
                     const humanizedErrors = error.error.map((err: any) => err.description);
                     // throw the array if needed elsewhere
                     throw humanizedErrors;
                  }
                  else if (error.error.errors) {
                     // for the rest of the cases ( traditional aspnet modal state error format)
                     const modalStateErrors = [];
                     for (const key in error.error.errors) {
                        if (error.error.errors[key]) {
                           modalStateErrors.push(error.error.errors[key]);
                        }
                     }
                     throw modalStateErrors.flat();
                  }
                  break;

               case 401:
                  toastr.error('Unauthorized', error.status)
                  break;

               case 403:
                  toastr.error('Forbidden', error.status)
                  break;

               case 404:
                  router.navigateByUrl('/not-found');
                  break;

               case 500:
                  const navigationExtras: NavigationExtras = { state: { error: error.error } };
                  router.navigateByUrl('/server-error', navigationExtras);
                  break;

               default:
                  toastr.error('Something unexpected went wrong');
                  break;
            }
         }
         throw error
      })
   );
};
