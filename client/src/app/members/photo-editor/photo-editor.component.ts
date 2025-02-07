import { Component, input, EventEmitter, Output, inject, output } from '@angular/core';
import { Member } from '../../_models/member';
import { AccountService } from '../../_services/account.service';
import { MembersService } from '../../_services/members.service';
import { Photo } from '../../_models/photo';
import { NgClass } from '@angular/common';




@Component({
   selector: 'app-photo-editor',
   standalone: true,
   imports: [NgClass],
   templateUrl: './photo-editor.component.html',
   styleUrl: './photo-editor.component.css'
})
export class PhotoEditorComponent {
   private memberService = inject(MembersService);
   private accountService = inject(AccountService);
   member = input.required<Member>();
   memberChanged = output<Member>();
   
   setMainPhoto(photo: Photo){
      this.memberService.setMainPhoto(photo).subscribe({
         next: _ => {
            const user = this.accountService.currentUser();
            if(user){
               user.photoUrl = photo.url;
               this.accountService.setCurrentUser(user);
            }

            const updatedMember = {...this.member()}
            updatedMember.photoUrl = photo.url;
            updatedMember.photos.forEach(p => {
               if(p.isMain)
                  p.isMain = false;
               if(p.id === photo.id)
                  p.isMain = true;
            });

            this.memberChanged.emit(updatedMember);  
         }
      });
   }

   deletePhoto(photo: Photo){
      this.memberService.deletePhoto(photo).subscribe({
         next: _ => {
            const updatedMember = {...this.member()};
            updatedMember.photos = updatedMember.photos.filter(x => x.id != photo.id);
            this.memberChanged.emit(updatedMember);
         }
      });
   }

};


