import { Injectable } from "@angular/core";
import { PagedResponse } from "../models/paged-response.model";
import { RecurringTransaction } from "../models/recurring.model";
import { ApiService } from "./api.service";

@Injectable({ providedIn: 'root' })
export class RecurringService {

  constructor(private api: ApiService) {}

  getAll() {
    return this.api.get<PagedResponse<RecurringTransaction>>('RecurringTransactions');
  }

  getById(id: number) {
    return this.api.get<RecurringTransaction>(`RecurringTransactions/${id}`);
  }

  create(model: any) {
    return this.api.post<RecurringTransaction>('RecurringTransactions', model);
  }

  update(id: number, model: any) {
    return this.api.put<RecurringTransaction>(`RecurringTransactions/${id}`, model);
  }

  delete(id: number) {
    return this.api.delete<void>(`RecurringTransactions/${id}`);
  }

  processDue() {
    return this.api.post<void>('RecurringTransactions/process-due', {});
  }
}