import { Component, OnInit } from '@angular/core';
import { BudgetService } from '../../core/services/budget.service';
import { CommonModule } from '@angular/common';
//Budget model
import { Budget } from '../../core/models/budget.model';
//router
import { Router } from '@angular/router';
import {PagedResponse} from '../../core/models/paged-response.model';

//paged response

@Component({
  standalone: true,
  selector: 'app-budgets',
  imports: [CommonModule],
  // templateUrl: './budgets.component.html'
   template: `
    <div class="header">
      <h2>Budgets</h2>
      <button (click)="goToCreate()">Add Budget</button>
    </div>

    <div *ngIf="isLoading" class="loading">
      Loading...
    </div>

    <div *ngIf="!isLoading && !budgets.length" class="empty">
      No budgets found.
    </div>

    <table *ngIf="!isLoading && budgets.length">
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
          <td>{{ budget.month }}</td>
          <td>
            <button (click)="edit(budget.id)">Edit</button>
            <button (click)="remove(budget.id)">Delete</button>
          </td>
        </tr>
      </tbody>
    </table>
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
  `]
})

export class BudgetsComponent implements OnInit {
  items: any[] = [];
  budgets: Budget[] = [];
  isLoading = false;

  constructor(private router: Router,
    private budgetService: BudgetService
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
    if (!confirm('Are you sure you want to delete this budget?')) return;

    this.budgetService.delete(id)
      .subscribe(() => this.loadBudgets());
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