import { Directive, inject, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Directive({
   selector: '[appHasRole]', // use: *appHasRole in html
   standalone: true
})
// directive used to remove the element from the DOM if the user lacks required roles
export class HasRoleDirective implements OnInit{
   @Input() appHasRole : string[] = [];
   private accountService = inject(AccountService);
   private viewContainerRef = inject(ViewContainerRef); // the container where the view (template) will be created or cleared
   private templateRef = inject(TemplateRef); //  blueprint of the element's content to be rendered if conditions are met
   
   ngOnInit(): void {
       // check if the user has at least one of the required roles
      if (this.accountService.roles()?.some((r:string) => this.appHasRole.includes(r))) {
         this.viewContainerRef.createEmbeddedView(this.templateRef) // render the template if the user has a required role
      } 
      else {
         this.viewContainerRef.clear(); // remove the element from the DOM if no required role is present
      }
   }
   
}
