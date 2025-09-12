import { inject, Injectable, signal } from '@angular/core';
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
  onlineUsers = signal<string[]>([]);

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

  // User comes online
  this.hubConnection.on('UserIsOnline', username => {
    console.log('[PresenceService] UserIsOnline event:', username);
    this.toastr.info(username + ' has connected');
    this.onlineUsers.set([...this.onlineUsers(), username]); // add user to signal
    console.log('[PresenceService] onlineUsers after UserIsOnline:', this.onlineUsers());

  });

  // User goes offline
  this.hubConnection.on('UserIsOffline', username => {
    console.log('[PresenceService] UserIsOffline event:', username);
    this.toastr.warning(username + ' has disconnected');
    this.onlineUsers.set(this.onlineUsers().filter(u => u !== username)); // remove user
    console.log('[PresenceService] onlineUsers after UserIsOffline:', this.onlineUsers());
  });

  // Initial list of online users
  this.hubConnection.on('GetOnlineUsers', usernames => {
    console.log('[PresenceService] GetOnlineUsers event:', usernames);
    this.onlineUsers.set(usernames); // set full list
    console.log('[PresenceService] onlineUsers updated:', this.onlineUsers());
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