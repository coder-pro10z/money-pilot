import { Component, OnInit } from '@angular/core';
import { BudgetService } from '../../core/services/budget.service';
import { CommonModule } from '@angular/common';
//Budget model
import { Budget } from '../../core/models/budget.model';
//router
import { Router } from '@angular/router';
import {PagedResponse} from '../../core/models/paged-response.model';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner.component';
import { ConfirmationService } from '../../shared/services/confirmation.service';
import { NotificationService } from '../../shared/services/notification.service';

//paged response

@Component({
  standalone: true,
  selector: 'app-budgets',
  imports: [CommonModule,LoadingSpinnerComponent],
  // templateUrl: './budgets.component.html'
   template: `
    <div class="header">
      <h2>Budgets</h2>
      <button class="btn btn-primary" (click)="goToCreate()">Add Budget</button>
    </div>

    
    <div *ngIf="!isLoading && !budgets.length" class="empty">
    No budgets found.
    </div>
    
    <app-loading-spinner *ngIf="isLoading"></app-loading-spinner>

    <div class="card" > 
    <table  class="data-table" *ngIf="!isLoading && budgets.length">
      <thead>
        <tr>
          <th>Category</th>
          <th>Monthly Limit</th>
          <th>Month</th>
          <th>Actions</th>
        </tr>
      </thead>

      <tbody>
        <tr *ngFor="let budget of budgets">
          <td>{{ budget.categoryName }}</td>
          <td>{{ budget.monthlyLimit | currency }}</td>
          <td>{{ budget.month |  date:'MMM yyyy'  }}</td>
          <td>
            <button class="btn" (click)="edit(budget.id)">Edit</button>
            <button class="btn btn-danger" (click)="remove(budget.id)">Delete</button>
          </td>
        </tr>
      </tbody>
    </table>
    </div>
  `,
  styles: [`
    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1rem;
    }

    table {
      width: 100%;
      border-collapse: collapse;
    }

    th, td {
      padding: 0.75rem;
      border-bottom: 1px solid #ddd;
    }

    button {
      margin-right: 0.5rem;
      cursor: pointer;
    }

    .loading, .empty {
      margin-top: 1rem;
    }

    /* Categories table: category, Monthly Limit, Month, Actions */
    .col-category { width: 25%; }
.col-limit { width: 20%; }
.col-month { width: 20%; }
.col-actions { width: 35%; }
  /* Category */
  /* Monthly Limit */
  /* Month */
  /* Actions */
  `]
})

export class BudgetsComponent implements OnInit {
  items: any[] = [];
  budgets: Budget[] = [];
  isLoading = false;

  constructor(private router: Router,
    private budgetService: BudgetService,
    private confirmationService: ConfirmationService,
    private notificationService: NotificationService
  ) {}
  ngOnInit(): void {
    this.loadBudgets();
  }

  loadBudgets() {
    this.isLoading = true;
    this.budgetService.getAll().subscribe(response => {
      console.log(response);
      this.budgets = response.items;
      this.isLoading = false;
    });
  }
      // this.budgets = response.items;
  /**
   * Navigate to create page
   */
  goToCreate() {
    this.router.navigate(['/budget/create']);
  }

  /**
   * Navigate to edit page
   */
  edit(id: number) {
    this.router.navigate(['/budget/edit', id]);
  }

  /**
   * Delete budget
   */
  remove(id: number) {
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

// Features:
// Table
// Category Name
// Monthly Limit (currency pipe)
// Month
// Edit
// Delete
// Add Budget button
// Loading state
// Empty state
