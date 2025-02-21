import { Component, computed, inject, input } from '@angular/core';
import { Member } from '../../_models/member';
import { RouterLink } from '@angular/router';
import { LikesService } from '../../_services/likes.service';
import { NgIf } from '@angular/common';

@Component({
   selector: 'app-member-card',
   standalone: true,
   imports: [RouterLink, NgIf],
   templateUrl: './member-card.component.html',
   styleUrl: './member-card.component.css'
})
export class MemberCardComponent {
   private _likeService = inject(LikesService);
   member = input.required<Member>();
   hasLiked = computed(() => this._likeService.likeIds().includes(this.member().id)); // check if the user in the card is liked by current user

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
}
