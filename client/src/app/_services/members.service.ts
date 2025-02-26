import { HttpClient, HttpHeaders, HttpParams, HttpResponse } from '@angular/common/http';
import { inject, Injectable, model, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { AccountService } from './account.service';
import { of, tap } from 'rxjs';
import { Photo } from '../_models/photo';
import { PageResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { setPaginatedResult, setPaginationHeaders } from '../_helpers/paginationHelper';

@Injectable({
   providedIn: 'root'
})
export class MembersService {
   private http = inject(HttpClient);
   baseUrl = environment.apiUrl;
   paginatedResult = signal<PageResult<Member[]> | null>(null);
   memberCache = new Map(); // map for caching responses for specific set userParams
   userParams = signal<UserParams>(new UserParams());


   getMembers() {
      // check if response exists for given params
      const response = this.memberCache.get(Object.values(this.userParams).join('-'))
      if (response) return setPaginatedResult(response, this.paginatedResult); // if response already in cache, load it from cache instead of api

      // set headers to use for filtering in api
      let params = setPaginationHeaders(this.userParams().pageNumber, this.userParams().pageSize);
      params = params.append('minAge', this.userParams().minAge);
      params = params.append('maxAge', this.userParams().maxAge);

      const userParams = this.userParams(); // save the current state before checks

      if (userParams?.gender !== undefined) {
         params = params.append('gender', userParams.gender);
      }

      if (userParams?.orderBy !== undefined) {
         params = params.append('orderBy', userParams.orderBy);
      }

      // make an api call if response not in cache
      return this.http.get<Member[]>(this.baseUrl + 'users', { observe: 'response', params }).subscribe({
         next: response => {
            setPaginatedResult(response, this.paginatedResult) // populare result variable with response data
            this.memberCache.set(Object.values(this.userParams()).join('-'), response); // save response in cache
         }
      })
   }



   resetUserParams() {
      this.userParams.set(new UserParams());
   }

   getMember(username: string) {
      // get the array of responses, find response bodies and append the bodies to empty array creating the array of user infos
      const member: Member = [...this.memberCache.values()]
         .reduce((arr, current) => arr.concat(current.body), [])
         .find((m: Member) => m.username === username);


      if (member) return of(member);

      return this.http.get<Member>(this.baseUrl + 'users/' + username); // if user not found in list make the api call
   }

   

   updateMember(member: Member) {
      return this.http.put(this.baseUrl + 'users', member).pipe(
         // tap(() => {
         //    this.members.update(members => members.map(m => m.username === member.username ? member : m))
         // })
      );
   }

   setMainPhoto(photo: Photo) {
      return this.http.put(this.baseUrl + 'users/set-main-photo/' + photo.id, {}).pipe(
         // tap(() => {
         //    this.members.update(members => members.map(m => {
         //       if (m.photos.includes(photo)) {
         //          m.photoUrl = photo.url;
         //       }
         //       return m;
         //    }))
         // })
      )
   }


   deletePhoto(photo: Photo) {
      return this.http.delete(this.baseUrl + 'users/delete-photo/' + photo.id).pipe(
         // tap(() => {
         //    this.members.update(members => members.map(m => {
         //       if (m.photos.includes(photo)){
         //          m.photos = m.photos.filter(x => x.id !== photo.id);
         //       }
         //       return m;

         //    }))
         // })
      )
   }
}
