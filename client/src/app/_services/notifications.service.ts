import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { User } from '../_models/user';



@Injectable({
  providedIn: 'root'
})
export class NotificationsService {
  private _hubUrl = environment.hubsUrl;
  private _hubConnection?: HubConnection;
  newlyLikedUserIds: number[] = [];
  likeNotificationsCount = 0;

  createHubConnection(token: string) {
    this._hubConnection = new HubConnectionBuilder()
      .withUrl(`${this._hubUrl}notifications`, {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

    this._hubConnection
      .start()
      .then(() => console.log('NotificationsHub connected'))
      .catch(err => console.log('Error while starting connection: ' + err));

    // listen for "ReceiveLikeNotification"
    this._hubConnection.on('ReceiveLikeNotification', data => {
      console.log('Received a like notification', data);
      const { sourceUserId } = data;
      if (!this.newlyLikedUserIds.includes(sourceUserId)) {
        this.newlyLikedUserIds.push(sourceUserId);
        this.likeNotificationsCount++;
      }
    });
  }

  stopHubConnection() {
    if (this._hubConnection?.state === HubConnectionState.Connected) {
      this._hubConnection.stop().catch(error => console.log(error));
    }
  }

  markNotificationsAsRead() {
    this.newlyLikedUserIds = [];
    this.likeNotificationsCount = 0;
  }

}
