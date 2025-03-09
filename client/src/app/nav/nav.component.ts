import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms'
import { AccountService } from '../_services/account.service';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { HasRoleDirective } from '../_directives/has-role.directive';
import { MessageService } from '../_services/message.service';
import { Message } from '../_models/message';
import { CommonModule } from '@angular/common';
import { LikesService } from '../_services/likes.service';
import { TimeagoModule } from 'ngx-timeago';
import { NotificationsService } from '../_services/notifications.service';


@Component({
   selector: 'app-nav',
   standalone: true,
   imports: [FormsModule, BsDropdownModule, RouterLink, RouterLinkActive, HasRoleDirective, CommonModule, TimeagoModule],
   templateUrl: './nav.component.html',
   styleUrl: './nav.component.css'
})
export class NavComponent implements OnInit {
   model: any = {};
   accountService = inject(AccountService);
   messageService = inject(MessageService);
   likesService = inject(LikesService);
   notificationsService = inject(NotificationsService);

   ngOnInit(): void {
      const user = this.accountService.currentUser();
      if (!user) return;

      this.notificationsService.createHubConnection(user.token)
   }

   fetchLatestMessages() {
      this.messageService.getLatestMessagesWithUniqueUsers();
      
   }

   fetchLatestLikeNotifications() {
      this.likesService.getLatestLikeNotifications();
   }

   onNotificationsTabHidden() {
      this.notificationsService.markNotificationsAsRead();
    }

   isUserInNotifications(userId: number): boolean {
      return this.notificationsService.newlyLikedUserIds.includes(userId);
   }

   logout() {
      this.accountService.logout()
   }

}
