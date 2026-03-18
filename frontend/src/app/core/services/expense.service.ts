import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Expense } from '../models/expense.model';
import { PagedResponse } from '../models/paged-response.model';
import { ApiResponse } from '../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class ExpenseService {

  constructor(private api: ApiService) {}

  getAll(page = 1, pageSize = 20) {
    return this.api.get<PagedResponse<Expense>>(`expense?page=${page}&pageSize=${pageSize}`);
  }

  getById(id: number) {
    
    return this.api.get<Expense>(`expense/${id}`);
  }

  create(model: Expense) {
    return this.api.post<Expense>('expense', model);
  }

  update(id: number, model: Expense) {
    return this.api.put<Expense>(`expense/${id}`, model);
  }

  delete(id: number) {
    return this.api.delete<void>(`expense/${id}`);
  }
}
