import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr'
import { ToastrService } from 'ngx-toastr';
import { User } from '../_models/user';
import { take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
   providedIn: 'root'
})
export class PresenceService {
   hubUrl = environment.hubsUrl;
   private _hubConnection?: HubConnection;
   private _toastr = inject(ToastrService);
   private _router = inject(Router);
   onlineUsers = signal<string[]>([]);

   createHubConnection(user: User) {
      // build hub connection
      this._hubConnection = new HubConnectionBuilder()
         .withUrl(this.hubUrl + 'presence', {
            accessTokenFactory: () => user.token
         })
         .withAutomaticReconnect()
         .build();
      // start connection
      this._hubConnection.start().catch(error => console.log(error));
      this._hubConnection.on('UserIsOnline', username => {
         this.onlineUsers.update(users => [...users, username]);
      })

      this._hubConnection.on('UserIsOffline', username => {
         this.onlineUsers.update(users => users.filter(x => x !== username));
      })

      this._hubConnection.on("GetOnlineUsers", usernames => {
         this.onlineUsers.set(usernames);
      })

      this._hubConnection.on("NewMessageReceived", ({username,knownAs}) => {
         this._toastr.info(knownAs + ' has sent you a new message. Click to see it.')
            .onTap.pipe(take(1)).subscribe(() => this._router.navigateByUrl('/members/' + username + '?tab=Messages'));
      })
   }

   stopHubConnection() {
      if (this._hubConnection?.state === HubConnectionState.Connected){
         this._hubConnection.stop().catch(error => console.log(error));
      }
   }
}

