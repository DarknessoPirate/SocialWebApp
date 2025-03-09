import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { PageResult } from '../_models/pagination';
import { Message } from '../_models/message';
import { setPaginatedResult, setPaginationHeaders } from '../_helpers/paginationHelper';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { User } from '../_models/user';
import { Group } from '../_models/group';

@Injectable({
   providedIn: 'root'
})
export class MessageService {
   baseUrl = environment.apiUrl;
   hubUrl = environment.hubsUrl;
   private _httpClient = inject(HttpClient);
   hubConnection?: HubConnection;
   paginatedResult = signal<PageResult<Message[]> | null>(null); // messages array
   messageThread = signal<Message[]>([]);
   latestMessages = signal<Message[]>([]);

   // create the connection to the messages api hub, that will send events if something happens
   createHubConnection(user: User, otherUsername: string) {
      this.hubConnection = new HubConnectionBuilder()
         .withUrl(this.hubUrl + 'message?user=' + otherUsername, {
            accessTokenFactory: () => user.token
         })
         .withAutomaticReconnect()
         .build();

      this.hubConnection.start().catch(error => console.log(error));

      this.hubConnection.on("ReceiveMessageThread", messages => {
         this.messageThread.set(messages);
      })

      this.hubConnection.on("NewMessage", message => {
         this.messageThread.update(messages => [...messages, message]) // append new message on event to the list of messages
      })

      this.hubConnection.on("UpdatedGroup", (group: Group) => {
         if (group.connections.some(x => x.username === otherUsername)) {
            this.messageThread.update(messages => {
               messages.forEach(message => {
                  if (!message.dateRead) {
                     message.dateRead = new Date(Date.now());
                  }
               })
               return messages;
            })
         }
      })
   }

   // stops the messages hub connection
   stopHubConnection() {
      if (this.hubConnection?.state === HubConnectionState.Connected) {
         this.hubConnection.stop().catch(error => console.log(error));
      }
   }

   // get either sent/received messages by setting the container string to "Inbox" or "Outbox"
   getMessages(pageNumber: number, pageSize: number, container: string) {
      let params = setPaginationHeaders(pageNumber, pageSize)
      params = params.append('Container', container);

      return this._httpClient.get<Message[]>(this.baseUrl + 'messages', { observe: 'response', params })
         .subscribe({
            next: response => setPaginatedResult(response, this.paginatedResult)
         })
   }

   // Fetch the message thread between two users from the api
   getMessageThread(username: string) {
      return this._httpClient.get<Message[]>(this.baseUrl + 'messages/thread/' + username)
   }

   getLatestMessagesWithUniqueUsers() {
      return this._httpClient.get<Message[]>(this.baseUrl + 'messages/latest-messages', { observe: 'response' })
         .subscribe({
            next: response => {
               if (response.body) {
                  const messages = response.body.sort((a, b) => 
                     new Date(b.messageSent).getTime() - new Date(a.messageSent).getTime()
                  ); // ✅ Sort newest to oldest
                  
                  this.latestMessages.set(messages); 
                  console.log("✅ latestMessages updated:", this.latestMessages());
               }
            },
            error: err => console.error("❌ Failed to fetch latest messages:", err)
         });
   }

   async sendMessage(username: string, content: string) {
      return this.hubConnection?.invoke('SendMessage', { recipientUsername: username, content })
   }

   deleteMessage(id: number) {
      return this._httpClient.delete(this.baseUrl + 'messages/' + id);
   }
}
