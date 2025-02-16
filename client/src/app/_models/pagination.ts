export interface PageDetails {
   currentPage: number;
   itemsPerPage: number;
   totalItems: number;
   totalPages: number;
}

export class PageResult<T> {
   items?: T; 
   pageDetails?: PageDetails

}