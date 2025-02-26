import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { MemberCardComponent } from "../member-card/member-card.component";
import { PaginationModule } from 'ngx-bootstrap/pagination'
import { UserParams } from '../../_models/userParams';
import { FormsModule } from '@angular/forms';
import { ButtonsModule } from 'ngx-bootstrap/buttons'
import { CommonModule } from '@angular/common';
import { ClickOutsideDirective } from '../../_directives/click-outside.directive';

@Component({
   selector: 'app-member-list',
   standalone: true,
   imports: [MemberCardComponent, PaginationModule, FormsModule, ButtonsModule, CommonModule, ClickOutsideDirective],
   templateUrl: './member-list.component.html',
   styleUrl: './member-list.component.css'
})
export class MemberListComponent implements OnInit {
   memberService = inject(MembersService);
   genderList = [{ value: 'male', display: "Male" }, { value: 'female', display: 'Female' }]
   showFilters = false;

   ngOnInit(): void {
      if (!this.memberService.paginatedResult())
         this.loadMembers();
      console.log(this.genderList);
   }

   loadMembers() {
      this.memberService.getMembers();
   }

   toggleFilters(event: Event) {
      event.stopPropagation(); // stops the event from reaching document level
      this.showFilters = !this.showFilters;
      console.log("Toggle Filters:", this.showFilters);
   }

   hideFilters() {
      this.showFilters = false;
      console.log("outside click")
   }

   resetFilters() {
      this.memberService.resetUserParams();
      this.loadMembers()
   }

   pageChanged(event: any) {
      if (this.memberService.userParams().pageNumber !== event.page) {
         this.memberService.userParams().pageNumber = event.page;
         this.loadMembers()
      }
   }

}
