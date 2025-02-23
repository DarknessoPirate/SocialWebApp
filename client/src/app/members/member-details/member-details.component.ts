import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute } from '@angular/router';
import { Member } from '../../_models/member';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TimeagoModule } from 'ngx-timeago';
import { DatePipe } from '@angular/common';
import { MemberMessagesComponent } from "../member-messages/member-messages.component";
import { Message } from '../../_models/message';
import { MessageService } from '../../_services/message.service';


@Component({
   selector: 'app-member-details',
   standalone: true,
   imports: [TabsModule, GalleryModule, TimeagoModule, DatePipe, MemberMessagesComponent],
   templateUrl: './member-details.component.html',
   styleUrl: './member-details.component.css'
})
export class MemberDetailsComponent implements OnInit {
   private memberService = inject(MembersService);
   private messageService = inject(MessageService);
   private route = inject(ActivatedRoute);
   @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent;
   activeTab?: TabDirective;
   member: Member = {} as Member; // will be provided by resolver
   images: GalleryItem[] = [];
   messages: Message[] = [];

   ngOnInit(): void {
      this.route.data.subscribe({
         next: data => {
            this.member = data['member']; // read the resolved data
            this.member && this.member.photos.map(p => {
               this.images.push(new ImageItem({ src: p.url, thumb: p.url }))
            })
         }
      })

      this.route.queryParams.subscribe({
         next: params => {
            params['tab'] && this.selectTab(params['tab'])
         }
      })
   }

   onTabActivated(data: TabDirective) {
      this.activeTab = data;
      if (this.activeTab.heading === 'Messages' && this.messages.length === 0 && this.member) {
         this.messageService.getMessageThread(this.member.username).subscribe({
            next: messages => this.messages = messages
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

   onUpdateMessages(event: Message){
      this.messages.push(event);
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
