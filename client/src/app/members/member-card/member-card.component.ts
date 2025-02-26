import { Component, computed, inject, input } from '@angular/core';
import { Member } from '../../_models/member';
import { RouterLink } from '@angular/router';
import { LikesService } from '../../_services/likes.service';
import { CommonModule, NgClass, NgIf } from '@angular/common';
import { PresenceService } from '../../_services/presence.service';

@Component({
   selector: 'app-member-card',
   standalone: true,
   imports: [RouterLink, NgIf, NgClass, CommonModule],
   templateUrl: './member-card.component.html',
   styleUrl: './member-card.component.css'
})
export class MemberCardComponent {
   private _likeService = inject(LikesService);
   private _presenceService = inject(PresenceService);
   member = input.required<Member>();
   hasLiked = computed(() => this._likeService.likeIds().includes(this.member().id)); // check if the user in the card is liked by current user
   isOnline = computed(() => this._presenceService.onlineUsers().includes(this.member().username));

   
   toggleLike() {
      this._likeService.toggleLike(this.member().id).subscribe({
         next: () => {
            if (this.hasLiked()) {
               this._likeService.likeIds.update(ids => ids.filter(x => x !== this.member().id))
            }
            else {
               this._likeService.likeIds.update(ids => [...ids, this.member().id])
            }
         }
      })
   }

   getBackgroundUrl() {
      const bgPhoto = this.member().photos?.find(photo => photo.isBackground);
      return bgPhoto ? bgPhoto.url : null;
   }
}
