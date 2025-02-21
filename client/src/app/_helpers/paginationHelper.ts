import { HttpParams, HttpResponse } from "@angular/common/http";
import { Member } from "../_models/member";
import { signal } from "@angular/core";
import { PageResult } from "../_models/pagination";

// moved the methods to reuse with multiple different objects 
export function setPaginatedResult<T>(response: HttpResponse<T>, paginatedResultSignal:ReturnType<typeof signal<PageResult<T> | null>>) { 
   paginatedResultSignal.set({
      items: response.body as T,
      pageDetails: JSON.parse(response.headers.get('Pagination')!)
   })
}

export function setPaginationHeaders(pageNumber: number, pageSize: number) {
   let params = new HttpParams();

   if (pageNumber && pageSize) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
   }

   return params
}