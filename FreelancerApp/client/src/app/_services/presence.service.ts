import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {

  hubUrl = environment.hubsUrl;
  private hubConnection?: HubConnection;
  private toastr = inject(ToastrService);

  createHubConnection(user: User) {
    console.log('[PresenceService] Creating Hub Connection for:', user.username);

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .then(() => console.log('[PresenceService] Hub connected'))
      .catch(error => console.error('[PresenceService] Error starting Hub:', error));

    this.hubConnection.on('UserIsOnline', username => {
      console.log('[PresenceService] UserIsOnline event:', username);
      this.toastr.info(username + ' has connected');
    });

    this.hubConnection.on('UserIsOffline', username => {
      console.log('[PresenceService] UserIsOffline event:', username);
      this.toastr.warning(username + ' has disconnected');
    });

    this.hubConnection.on('GetOnlineUsers', usernames => {
      console.log('[PresenceService] GetOnlineUsers event:', usernames);
    });
  }

  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      console.log('[PresenceService] Stopping Hub Connection');
      this.hubConnection.stop()
        .then(() => console.log('[PresenceService] Hub stopped'))
        .catch(error => console.error('[PresenceService] Error stopping Hub:', error));
    } else {
      console.log('[PresenceService] Hub is not connected, nothing to stop');
    }
  }
}