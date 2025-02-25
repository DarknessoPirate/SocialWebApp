import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { User } from '../_models/user';

@Injectable({
   providedIn: 'root'
})
export class AdminService {
   baseUrl = environment.apiUrl;
   private _httpClient = inject(HttpClient)

   getUserWithRoles() {
      return this._httpClient.get<User[]>(this.baseUrl + 'admin/get-users-with-roles')
   }

   updateUserRoles(username: string, roles: string[]){
      return this._httpClient.post<string[]>(this.baseUrl + 'admin/edit-roles/' + username + '?roles=' + roles, {})
   }
}
