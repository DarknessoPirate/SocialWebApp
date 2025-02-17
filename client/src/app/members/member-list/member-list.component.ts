import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { MemberCardComponent } from "../member-card/member-card.component";
import { PaginationModule } from 'ngx-bootstrap/pagination'
import { UserParams } from '../../_models/userParams';
import { FormsModule } from '@angular/forms';
import {ButtonsModule} from 'ngx-bootstrap/buttons'

@Component({
   selector: 'app-member-list',
   standalone: true,
   imports: [MemberCardComponent, PaginationModule, FormsModule, ButtonsModule],
   templateUrl: './member-list.component.html',
   styleUrl: './member-list.component.css'
})
export class MemberListComponent implements OnInit {
   private cdRef = inject(ChangeDetectorRef);
   memberService = inject(MembersService);
   userParams = new UserParams();
   genderList = [{ value: 'male', display: "Male" }, { value: 'female', display: 'Female' }]

   ngOnInit(): void {
      if (!this.memberService.paginatedResult())
         this.loadMembers();
   }

   loadMembers() {
      this.cdRef.detectChanges();
      this.memberService.getMembers(this.userParams);
   }

   resetFilters() {
      this.userParams = new UserParams();
      this.loadMembers()
   }

   pageChanged(event: any) {
      if (this.userParams.pageNumber !== event.page) {
         this.userParams.pageNumber = event.page;
         this.loadMembers()
      }
   }

   updateOrderBy(orderBy: string) {
      console.log("Before change:", this.userParams.orderBy);
      this.userParams.orderBy = orderBy;
      console.log("After change:", this.userParams.orderBy);
      this.loadMembers();
   }
   
}
