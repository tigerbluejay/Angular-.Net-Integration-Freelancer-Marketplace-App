export interface Pagination2 {
  currentPage: number;
  itemsPerPage: number;
  totalItems: number;
  totalPages: number;
}

export class PaginatedResult2<T> {
  result!: T[]; // ← note the array here
  pagination!: Pagination2;
}