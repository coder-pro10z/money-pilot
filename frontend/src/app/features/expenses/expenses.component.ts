import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Expense } from '../../core/models/expense.model';
import { ExpenseService } from '../../core/services/expense.service';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner.component';
import { ConfirmationService } from '../../shared/services/confirmation.service';
import { NotificationService } from '../../shared/services/notification.service';

@Component({
  selector: 'app-expenses',
  imports: [CommonModule, FormsModule, LoadingSpinnerComponent],
  standalone: true,
  template: `
    <div class="header">
      <h2 class="page-title">Expenses</h2>
      <button class="btn btn-primary" (click)="goToCreate()">Add Expense</button>
    </div>

    <div class="toolbar" *ngIf="expenses.length">
      <input [(ngModel)]="searchTerm" (ngModelChange)="resetPagination()" type="text" placeholder="Search description or category" />

      <select [(ngModel)]="categoryFilter" (ngModelChange)="resetPagination()">
        <option value="all">All Categories</option>
        <option *ngFor="let category of categoryOptions" [value]="category">{{ category }}</option>
      </select>

      <select [(ngModel)]="sortBy" (ngModelChange)="resetPagination()">
        <option value="latest">Latest First</option>
        <option value="oldest">Oldest First</option>
        <option value="high">Highest Amount</option>
        <option value="low">Lowest Amount</option>
      </select>
    </div>

    <app-loading-spinner *ngIf="isLoading"></app-loading-spinner>

    <div class="empty-state" *ngIf="!isLoading && !expenses.length">
      <h3>No expenses yet</h3>
      <p>Start tracking your spending by adding your first expense.</p>
      <button class="btn btn-primary" (click)="goToCreate()">Add Your First Expense</button>
    </div>

    <div class="empty-state" *ngIf="!isLoading && expenses.length && !filteredExpenses.length">
      <h3>No matching expenses</h3>
      <p>Try a different search or clear the filters.</p>
      <button class="btn" (click)="clearFilters()">Clear Filters</button>
    </div>

    <div class="card" *ngIf="filteredExpenses.length">
      <table class="data-table">
        <thead>
          <tr>
            <th>Description</th>
            <th>Amount</th>
            <th>Date</th>
            <th>Category</th>
            <th>Actions</th>
          </tr>
        </thead>

        <tbody>
          <tr *ngFor="let expense of pagedExpenses">
            <td>{{ expense.description }}</td>
            <td>{{ expense.amount | currency }}</td>
            <td>{{ expense.date | date:'mediumDate' }}</td>
            <td>{{ expense.categoryName }}</td>
            <td>
              <button class="btn" (click)="edit(expense.id)">Edit</button>
              <button class="btn btn-danger" (click)="confirmDelete(expense.id)">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>

      <div class="pagination" *ngIf="totalPages > 1">
        <button class="btn" (click)="previousPage()" [disabled]="currentPage === 1">Previous</button>
        <span>Page {{ currentPage }} of {{ totalPages }}</span>
        <button class="btn" (click)="nextPage()" [disabled]="currentPage === totalPages">Next</button>
      </div>
    </div>
  `,
  styles: [`
    table {
      width: 100%;
      border-collapse: collapse;
      margin-top: 1rem;
    }

    th, td {
      padding: 0.75rem;
      text-align: left;
      border-bottom: 1px solid #ddd;
    }

    th {
      background-color: #f5f5f5;
    }

    button {
      margin-right: 0.5rem;
    }

    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1rem;
    }

    .toolbar {
      display: grid;
      grid-template-columns: 2fr 1fr 1fr;
      gap: 12px;
      margin-bottom: 16px;
    }

    .toolbar input,
    .toolbar select {
      padding: 10px 12px;
      border: 1px solid #d0d5dd;
      border-radius: 8px;
      background: #fff;
    }

    .empty-state {
      background: #ffffff;
      border: 1px dashed #d0d5dd;
      border-radius: 12px;
      padding: 32px 24px;
      text-align: center;
      color: #344054;
    }

    .empty-state h3 {
      margin: 0 0 8px;
      color: #101828;
    }

    .empty-state p {
      margin: 0 0 16px;
    }

    .pagination {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 16px 0 8px;
    }

    .data-table th:nth-child(1) { width: 25%; }
    .data-table th:nth-child(2) { width: 15%; }
    .data-table th:nth-child(3) { width: 15%; }
    .data-table th:nth-child(4) { width: 20%; }
    .data-table th:nth-child(5) { width: 25%; }

    @media (max-width: 768px) {
      .toolbar {
        grid-template-columns: 1fr;
      }

      .pagination {
        flex-direction: column;
        gap: 12px;
      }
    }
  `]
})
export class ExpensesComponent implements OnInit {
  expenses: Expense[] = [];
  isLoading = false;
  searchTerm = '';
  categoryFilter = 'all';
  sortBy = 'latest';
  currentPage = 1;
  readonly pageSize = 5;

  constructor(
    private expenseService: ExpenseService,
    private router: Router,
    private confirmationService: ConfirmationService,
    private notificationService: NotificationService
  ) {}

  get categoryOptions(): string[] {
    return [...new Set(this.expenses.map(expense => expense.categoryName).filter(Boolean))].sort();
  }

  get filteredExpenses(): Expense[] {
    const term = this.searchTerm.trim().toLowerCase();
    const items = this.expenses.filter(expense => {
      const matchesSearch =
        !term ||
        expense.description.toLowerCase().includes(term) ||
        expense.categoryName?.toLowerCase().includes(term);

      const matchesCategory =
        this.categoryFilter === 'all' || expense.categoryName === this.categoryFilter;

      return matchesSearch && matchesCategory;
    });

    return items.sort((a, b) => {
      switch (this.sortBy) {
        case 'oldest':
          return new Date(a.date).getTime() - new Date(b.date).getTime();
        case 'high':
          return b.amount - a.amount;
        case 'low':
          return a.amount - b.amount;
        default:
          return new Date(b.date).getTime() - new Date(a.date).getTime();
      }
    });
  }

  get pagedExpenses(): Expense[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.filteredExpenses.slice(start, start + this.pageSize);
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.filteredExpenses.length / this.pageSize));
  }

  ngOnInit(): void {
    this.loadExpenses();
  }

  loadExpenses(): void {
    this.isLoading = true;
    this.expenseService.getAll(1, 100).subscribe(response => {
      this.expenses = response.items;
      this.currentPage = 1;
      this.isLoading = false;
    });
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.categoryFilter = 'all';
    this.sortBy = 'latest';
    this.currentPage = 1;
  }

  resetPagination(): void {
    this.currentPage = 1;
  }

  goToCreate(): void {
    this.router.navigate(['expense/create']);
  }

  edit(id: number): void {
    this.router.navigate(['expense/edit', id]);
  }

  previousPage(): void {
    this.currentPage = Math.max(1, this.currentPage - 1);
  }

  nextPage(): void {
    this.currentPage = Math.min(this.totalPages, this.currentPage + 1);
  }

  confirmDelete(id: number): void {
    this.confirmationService.confirm({
      title: 'Delete Expense',
      message: 'Are you sure you want to delete this expense? This action cannot be undone.',
      confirmText: 'Delete',
      cancelText: 'Cancel'
    }).subscribe(confirmed => {
      if (!confirmed) {
        return;
      }

      this.expenseService.delete(id).subscribe({
        next: () => {
          this.notificationService.success('Expense deleted successfully.');
          this.loadExpenses();
        }
      });
    });
  }
}
