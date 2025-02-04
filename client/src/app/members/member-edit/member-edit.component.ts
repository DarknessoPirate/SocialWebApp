import { Component, HostListener, inject, OnInit, ViewChild } from '@angular/core';
import { AccountService } from '../../_services/account.service';
import { Member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { PhotoEditorComponent } from '../photo-editor/photo-editor.component';
import { PhotoUploaderComponent } from "../photo-uploader/photo-uploader.component";

@Component({
   selector: 'app-member-edit',
   standalone: true,
   imports: [TabsModule, FormsModule, PhotoEditorComponent, PhotoUploaderComponent],
   templateUrl: './member-edit.component.html',
   styleUrl: './member-edit.component.css'
})
export class MemberEditComponent implements OnInit {
   @ViewChild('editForm') editForm?: NgForm; // editform handle
   @HostListener('window:beforeunload', ['$event']) notify($event: any) {  // used to prevent closing/refreshing/leaving page when changes unsaved
      if (this.editForm?.dirty) {
         $event.returnValue = true;
      }
   }
   member?: Member;
   private accountService = inject(AccountService);
   private memberService = inject(MembersService);
   private toastr = inject(ToastrService);

   ngOnInit(): void {
      this.loadMember()
   }

   loadMember() {
      const user = this.accountService.currentUser();
      if (!user) return;

      this.memberService.getMember(user.username).subscribe({
         next: member => this.member = member
      })
      //console.log("------------ LOADING MEMBER -----------------")
   }

   updateMember() {
      this.memberService.updateMember(this.editForm?.value).subscribe({
         next: _ => {
            this.toastr.success('Profile updated successfully')
            this.editForm?.reset(this.member);
         }
      })

   }
}
