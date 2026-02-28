import { Injectable } from "@angular/core";
import { ApiService } from "./api.service";
import { Budget } from "../models/budget.model";
import { PagedResponse } from "../models/paged-response.model";

@Injectable({
  providedIn: 'root'
})
export class BudgetService {

  constructor(private api: ApiService) {}

  getAll() {
    return this.api.get<PagedResponse<Budget>>('budget');
  }

  getById(id: number) {
    return this.api.get<Budget>(`budget/${id}`);
  }

  create(model: Budget) {
    return this.api.post<Budget>('budget', model);
  }

  update(id: number, model: Budget) {
    return this.api.put<Budget>(`budget/${id}`, model);
  }

  delete(id: number) {
    return this.api.delete<void>(`budget/${id}`);
  }
}