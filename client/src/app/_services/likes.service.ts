import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Member } from '../_models/member';
import { PageResult } from '../_models/pagination';
import { setPaginatedResult, setPaginationHeaders } from '../_helpers/paginationHelper';

@Injectable({
   providedIn: 'root'
})
export class LikesService {
   baseUrl = environment.apiUrl;
   private _httpClient = inject(HttpClient);
   likeIds = signal<number[]>([]);
   paginatedResult = signal<PageResult<Member[]> | null>(null);

   toggleLike(targetId: number) {
      return this._httpClient.post(`${this.baseUrl}likes/${targetId}`, {})
   }

   getLikes(predicate: string, pageNumber: number, pageSize: number) {
      let params = setPaginationHeaders(pageNumber, pageSize);
      params = params.append('predicate', predicate)
      return this._httpClient.get<Member[]>(`${this.baseUrl}likes`, 
         {observe: 'response', params}).subscribe({
            next: response => setPaginatedResult(response, this.paginatedResult)
         })
   }

   getLikeIds() {
      return this._httpClient.get<number[]>(`${this.baseUrl}likes/list`).subscribe({
         next: ids => this.likeIds.set(ids)
      });
   }


}
