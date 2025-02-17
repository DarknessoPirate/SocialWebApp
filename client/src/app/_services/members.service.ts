import { HttpClient, HttpHeaders, HttpParams, HttpResponse } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { AccountService } from './account.service';
import { of, tap } from 'rxjs';
import { Photo } from '../_models/photo';
import { PageResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';

@Injectable({
   providedIn: 'root'
})
export class MembersService {
   private http = inject(HttpClient);
   baseUrl = environment.apiUrl;
   paginatedResult = signal<PageResult<Member[]> | null>(null);
   memberCache = new Map(); // map for caching responses for specific set userParams

   getMembers(userParams: UserParams) {
      // check if response exists for given params
      const response = this.memberCache.get(Object.values(userParams).join('-'))
      if (response) return this.setPaginatedResult(response); // if response already in cache, load it from cache instead of api
      
      // set headers to use for filtering in api
      let params = this.setPaginationHeaders(userParams.pageNumber, userParams.pageSize);
      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);

      if(userParams.gender)
         params = params.append('gender', userParams.gender);

      if(userParams.orderBy)
         params = params.append('orderBy', userParams.orderBy);

      // make an api call if response not in cache
      return this.http.get<Member[]>(this.baseUrl + 'users', { observe: 'response', params }).subscribe({
         next: response => {
            this.setPaginatedResult(response) // populare result variable with response data
            this.memberCache.set(Object.values(userParams).join('-'), response); // save response in cache
         }
      })
   }

   private setPaginatedResult(response: HttpResponse<Member[]>){
      this.paginatedResult.set({
         items: response.body as Member[],
         pageDetails: JSON.parse(response.headers.get('Pagination')!)
      })
   }

   private setPaginationHeaders(pageNumber: number, pageSize: number) {
      let params = new HttpParams();

      if (pageNumber && pageSize) {
         params = params.append('pageNumber', pageNumber);
         params = params.append('pageSize', pageSize);
      }

      return params
   }

   getMember(username: string) {
      // const member = this.members().find(x => x.username == username)
      // if (member !== undefined) return of(member); // return member as observable

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
