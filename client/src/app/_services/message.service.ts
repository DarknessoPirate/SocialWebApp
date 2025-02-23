import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { PageResult } from '../_models/pagination';
import { Message } from '../_models/message';
import { setPaginatedResult, setPaginationHeaders } from '../_helpers/paginationHelper';

@Injectable({
   providedIn: 'root'
})
export class MessageService {
   baseUrl = environment.apiUrl;
   private _httpClient = inject(HttpClient)
   paginatedResult = signal<PageResult<Message[]> | null>(null); // messages array

   getMessages(pageNumber: number, pageSize: number, container: string) {
      let params = setPaginationHeaders(pageNumber,pageSize)
      params = params.append('Container', container);

      return this._httpClient.get<Message[]>(this.baseUrl + 'messages', {observe: 'response', params})
      .subscribe({
         next: response => setPaginatedResult(response, this.paginatedResult)
      })
   }

   getMessageThread(username: string){
      return this._httpClient.get<Message[]>(this.baseUrl + 'messages/thread/' + username)
   }

   sendMessage(username: string, content: string){
      return this._httpClient.post<Message>(this.baseUrl + 'messages', {recipientUsername: username, content })
   }

   deleteMessage(id: number) {
      return this._httpClient.delete(this.baseUrl + 'messages/' + id);
   }
}
