import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms'
import { AccountService } from '../_services/account.service';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { HasRoleDirective } from '../_directives/has-role.directive';


@Component({
   selector: 'app-nav',
   standalone: true,
   imports: [FormsModule, BsDropdownModule, RouterLink, RouterLinkActive, HasRoleDirective],
   templateUrl: './nav.component.html',
   styleUrl: './nav.component.css'
})
export class NavComponent {
   model: any = {};
   accountService = inject(AccountService);
   private _router = inject(Router);
   private _toastr = inject(ToastrService)
   

   login() {
      this.accountService.login(this.model).subscribe({
         next: response => {
            console.log(response);
            this._router.navigateByUrl('/members');
         },
         error: error => this._toastr.error(error.error)
      })
   }

   logout() {
      this.accountService.logout()
   }

}
