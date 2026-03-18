import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Budget } from '../../core/models/budget.model';
import { BudgetService } from '../../core/services/budget.service';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner.component';
import { ConfirmationService } from '../../shared/services/confirmation.service';
import { NotificationService } from '../../shared/services/notification.service';

@Component({
  standalone: true,
  selector: 'app-budgets',
  imports: [CommonModule, FormsModule, LoadingSpinnerComponent],
  template: `
    <div class="header">
      <h2>Budgets</h2>
      <button class="btn btn-primary" (click)="goToCreate()">Add Budget</button>
    </div>

    <div class="toolbar" *ngIf="budgets.length">
      <input [(ngModel)]="searchTerm" (ngModelChange)="resetPagination()" type="text" placeholder="Search by category" />
      <select [(ngModel)]="sortBy" (ngModelChange)="resetPagination()">
        <option value="latest">Latest Month</option>
        <option value="oldest">Oldest Month</option>
        <option value="high">Highest Limit</option>
        <option value="low">Lowest Limit</option>
      </select>
    </div>

    <div *ngIf="!isLoading && !budgets.length" class="empty-state">
      <h3>No budgets yet</h3>
      <p>Create a budget to track spending limits and stay on target.</p>
      <button class="btn btn-primary" (click)="goToCreate()">Create Your First Budget</button>
    </div>

    <div *ngIf="!isLoading && budgets.length && !filteredBudgets.length" class="empty-state">
      <h3>No matching budgets</h3>
      <p>Adjust your search or sort to find the budget you want.</p>
      <button class="btn" (click)="clearFilters()">Clear Filters</button>
    </div>
    
    <app-loading-spinner *ngIf="isLoading"></app-loading-spinner>

    <div class="card" *ngIf="filteredBudgets.length"> 
      <table class="data-table">
        <thead>
          <tr>
            <th>Category</th>
            <th>Monthly Limit</th>
            <th>Month</th>
            <th>Actions</th>
          </tr>
        </thead>

        <tbody>
          <tr *ngFor="let budget of pagedBudgets">
            <td>{{ budget.categoryName }}</td>
            <td>{{ budget.monthlyLimit | currency }}</td>
            <td>{{ budget.month | date:'MMM yyyy' }}</td>
            <td>
              <button class="btn" (click)="edit(budget.id)">Edit</button>
              <button class="btn btn-danger" (click)="remove(budget.id)">Delete</button>
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
    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1rem;
    }

    .toolbar {
      display: grid;
      grid-template-columns: 2fr 1fr;
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
      margin-top: 1rem;
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
export class BudgetsComponent implements OnInit {
  budgets: Budget[] = [];
  isLoading = false;
  searchTerm = '';
  sortBy = 'latest';
  currentPage = 1;
  readonly pageSize = 5;

  constructor(
    private router: Router,
    private budgetService: BudgetService,
    private confirmationService: ConfirmationService,
    private notificationService: NotificationService
  ) {}

  get filteredBudgets(): Budget[] {
    const term = this.searchTerm.trim().toLowerCase();
    const items = this.budgets.filter(budget => !term || budget.categoryName?.toLowerCase().includes(term));

    return items.sort((a, b) => {
      switch (this.sortBy) {
        case 'oldest':
          return new Date(a.month).getTime() - new Date(b.month).getTime();
        case 'high':
          return b.monthlyLimit - a.monthlyLimit;
        case 'low':
          return a.monthlyLimit - b.monthlyLimit;
        default:
          return new Date(b.month).getTime() - new Date(a.month).getTime();
      }
    });
  }

  get pagedBudgets(): Budget[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.filteredBudgets.slice(start, start + this.pageSize);
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.filteredBudgets.length / this.pageSize));
  }

  ngOnInit(): void {
    this.loadBudgets();
  }

  loadBudgets(): void {
    this.isLoading = true;
    this.budgetService.getAll().subscribe(response => {
      this.budgets = response.items;
      this.currentPage = 1;
      this.isLoading = false;
    });
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.sortBy = 'latest';
    this.currentPage = 1;
  }

  resetPagination(): void {
    this.currentPage = 1;
  }

  goToCreate(): void {
    this.router.navigate(['/budget/create']);
  }

  edit(id: number): void {
    this.router.navigate(['/budget/edit', id]);
  }

  previousPage(): void {
    this.currentPage = Math.max(1, this.currentPage - 1);
  }

  nextPage(): void {
    this.currentPage = Math.min(this.totalPages, this.currentPage + 1);
  }

  remove(id: number): void {
    this.confirmationService.confirm({
      title: 'Delete Budget',
      message: 'Are you sure you want to delete this budget? This action cannot be undone.',
      confirmText: 'Delete',
      cancelText: 'Cancel'
    }).subscribe(confirmed => {
      if (!confirmed) {
        return;
      }

      this.budgetService.delete(id).subscribe(() => {
        this.notificationService.success('Budget deleted successfully.');
        this.loadBudgets();
      });
    });
  }
}
