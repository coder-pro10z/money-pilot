import { Component, OnInit } from '@angular/core';
import { ExpenseService } from '../../core/services/expense.service';
import { Expense } from '../../core/models/expense.model';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-expenses',
  imports: [CommonModule], 
  standalone: true,
  template: `
    <h2>Expenses</h2>

    <button (click)="goToCreate()">Add Expense</button>

    <div *ngIf="isLoading">Loading...</div>
    <table *ngIf="expenses.length">
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
            <button (click)="edit(expense.id)">Edit</button>
            <button (click)="remove(expense.id)">Delete</button>
          </td>
        </tr>
      </tbody>
    </table>
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
`]})
export class ExpensesComponent implements OnInit {

  expenses: Expense[] = [];
  // free: any[] = [];
 isLoading = false;
  constructor(
    private expenseService: ExpenseService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadExpenses();
  }

  // loadExpenses() {
  //   this.expenseService.getAll().subscribe(data => {
  //     this.expenses = data;
  //   });
  // }

//   loadExpenses() {
//   this.expenseService.getAll().subscribe(response => {
//      response; // Adjust based on your API response structure
//   });
// }

loadExpenses() {
  //set loading state
  this.isLoading = true;

  this.expenseService.getAll().subscribe(response => {
    this.expenses = response.items;
    //clear loading state
    this.isLoading = false;
    console.log(response);
    console.log(response.items);
  });
}
  goToCreate() {
    this.router.navigate(['expense/create']);
  }

  edit(id: number) {
    this.router.navigate(['expense/edit', id]);
  }

  remove(id: number) {
    if (confirm('Are you sure?')) {
      this.expenseService.delete(id).subscribe(() => {
        this.loadExpenses();
      });
    }
  }
}