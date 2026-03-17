import { Component, OnInit } from '@angular/core';
import { ExpenseService } from '../../core/services/expense.service';
import { Expense } from '../../core/models/expense.model';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner.component';
import { ConfirmationService } from '../../shared/services/confirmation.service';
import { NotificationService } from '../../shared/services/notification.service';

@Component({
  selector: 'app-expenses',
  imports: [CommonModule,LoadingSpinnerComponent], 
  standalone: true,
  template: `

    <div class="header">
    <h2 class="page-title">Expenses</h2>
    <button class="btn btn-primary" (click)="goToCreate()">Add Expense</button>
    </div>

    <app-loading-spinner *ngIf="isLoading"></app-loading-spinner>
    
    <div class="card" *ngIf="expenses.length">
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
        <tr *ngFor="let expense of expenses">
          <td>{{ expense.description }}</td>
          <td>{{ expense.amount | currency }}</td>
          <td>{{ expense.date  | date:'mediumDate'  }}</td>
          <td>{{ expense.categoryName }}</td>
          <td>
            <button class="btn" (click)="edit(expense.id)">Edit</button>
            <button class="btn btn-danger" (click)="confirmDelete(expense.id)">Delete</button>
          </td>
        </tr>
      </tbody>
    </table>
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

  /* Column widths for Expenses table */
.data-table th:nth-child(1) { width: 25%; }  /* Description */
.data-table th:nth-child(2) { width: 15%; }  /* Amount */
.data-table th:nth-child(3) { width: 15%; }  /* Date */
.data-table th:nth-child(4) { width: 20%; }  /* Category */
.data-table th:nth-child(5) { width: 25%; }  /* Actions (two buttons) */

`]})
export class ExpensesComponent implements OnInit {

  expenses: Expense[] = [];
  // free: any[] = [];
 isLoading = false;
  constructor(
    private expenseService: ExpenseService,
    private router: Router,
    private confirmationService: ConfirmationService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadExpenses();
  }


loadExpenses() {
  //set loading state
  this.isLoading = true;

  this.expenseService.getAll().subscribe(response => {
    this.expenses = response.items;
    //clear loading state
    this.isLoading = false;
    // console.log(response);
    // console.log(response.items);
  });
}
  goToCreate() {
    this.router.navigate(['expense/create']);
  }

  edit(id: number) {
    this.router.navigate(['expense/edit', id]);
  }

   confirmDelete(id: number) {
    this.confirmationService.confirm({
      title: 'Delete Expense',
      message: 'Are you sure you want to delete this expense? This action cannot be undone.',
      confirmText: 'Delete',
      cancelText: 'Cancel'
    }).subscribe(confirmed => {
      if (confirmed) {
        this.expenseService.delete(id).subscribe({
          next: () => {
            this.notificationService.success('Expense deleted successfully.');
            this.loadExpenses();
          },
          error: (err) => console.error('Delete failed', err)
        });
      }
    });
  }
}
