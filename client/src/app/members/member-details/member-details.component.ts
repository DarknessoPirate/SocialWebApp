import { Component, inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Member } from '../../_models/member';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TimeagoModule } from 'ngx-timeago';
import { DatePipe } from '@angular/common';
import { MemberMessagesComponent } from "../member-messages/member-messages.component";
import { Message } from '../../_models/message';
import { MessageService } from '../../_services/message.service';
import { PresenceService } from '../../_services/presence.service';
import { AccountService } from '../../_services/account.service';
import { HubConnection, HubConnectionState } from '@microsoft/signalr';


@Component({
   selector: 'app-member-details',
   standalone: true,
   imports: [TabsModule, GalleryModule, TimeagoModule, DatePipe, MemberMessagesComponent],
   templateUrl: './member-details.component.html',
   styleUrl: './member-details.component.css'
})
export class MemberDetailsComponent implements OnInit, OnDestroy {
   private _messageService = inject(MessageService);
   private _accountService = inject(AccountService);
   private _route = inject(ActivatedRoute);
   private _router = inject(Router);
   presenceService = inject(PresenceService);
   @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent;
   activeTab?: TabDirective;
   member: Member = {} as Member; // will be provided by resolver
   images: GalleryItem[] = [];

   ngOnInit(): void {
      this._route.data.subscribe({
         next: data => {
            this.member = data['member']; // read the resolved data
            this.member && this.member.photos.map(p => {
               this.images.push(new ImageItem({ src: p.url, thumb: p.url }))
            })
         }
      })

      this._route.paramMap.subscribe({
         next: _ => this.onRouteParamsChange()
      })

      this._route.queryParams.subscribe({
         next: params => {
            params['tab'] && this.selectTab(params['tab'])
         }
      })
   }

   ngOnDestroy(): void {
      this._messageService.stopHubConnection();
   }


   onTabActivated(data: TabDirective) {
      this.activeTab = data;
      this._router.navigate([], {
         relativeTo: this._route,
         queryParams: { tab: this.activeTab.heading },
         queryParamsHandling: 'merge'
      })
      if (this.activeTab.heading === 'Messages' && this.member) {
         const user = this._accountService.currentUser();
         if (!user) return;
         this._messageService.createHubConnection(user, this.member.username);
      }
      else {
         this._messageService.stopHubConnection();
      }
   }

   onRouteParamsChange() {
      const user = this._accountService.currentUser();
      if (!user)
         return;
      if (this._messageService.hubConnection?.state === HubConnectionState.Connected && this.activeTab?.heading === 'Messages') {
         this._messageService.hubConnection.stop().then(() => {
            this._messageService.createHubConnection(user, this.member.username);
         })
      }
   }

   selectTab(heading: string) {
      // check if tabs array is populated
      if (this.memberTabs) {
         // find the tab to switch to
         const messageTab = this.memberTabs.tabs.find(t => t.heading === heading);
         // check if tab found
         if (messageTab)
            messageTab.active = true; // switch to the tab
      }
   }


   // loadMember() {
   //    const username = this.route.snapshot.paramMap.get('username');

   //    if (!username)
   //       return;

   //    this.memberService.getMember(username).subscribe({
   //       next: member => {
   //          this.member = member
   //          member.photos.map(p => {
   //             this.images.push(new ImageItem({ src: p.url, thumb: p.url }))
   //          }) // Create new ImageItems of photos and add them to the array
   //       }
   //    })

   // }

}
