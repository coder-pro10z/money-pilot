import { Injectable } from "@angular/core";
import { ApiService } from "./api.service";
import { Budget } from "../models/budget.model";
import { PagedResponse } from "../models/paged-response.model";
import { CreateBudgetDto } from "../models/budget-create.model";

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

  create(model: CreateBudgetDto   ) {
    return this.api.post<Budget>('budget', model);
  }

  update(id: number, model: CreateBudgetDto) {
    return this.api.put<Budget>(`budget/${id}`, model);
  }

  delete(id: number) {
    return this.api.delete<void>(`budget/${id}`);
  }
}