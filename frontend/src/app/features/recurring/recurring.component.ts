import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { RecurringService } from '../../core/services/recurring.service';
import { RecurringTransaction } from '../../core/models/recurring.model';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner.component';

@Component({
  selector: 'app-recurring',
  standalone: true,
  imports: [CommonModule,LoadingSpinnerComponent],
  template: `
    <h2>Recurring Transactions</h2>

    <div class="actions">
      <button (click)="goToCreate()">Add Recurring</button>
      <button class="run" (click)="runNow()">Run Due Now</button>
    </div>

    <app-loading-spinner *ngIf="isLoading"></app-loading-spinner>

    <table *ngIf="!isLoading && recurringList.length">
      <thead>
        <tr>
          <th>Description</th>
          <th>Amount</th>
          <th>Category</th>
          <th>Type</th>
          <th>Start</th>
          <th>End</th>
          <th>Active</th>
          <th>Actions</th>
        </tr>
      </thead>

      <tbody>
        <tr *ngFor="let item of recurringList">
          <td>{{ item.description }}</td>
          <td>{{ item.amount | currency }}</td>
          <td>{{ item.categoryName }}</td>
          <td>{{ getRecurrenceLabel(item.recurrenceType) }}</td>
          <td>{{ item.startDate | date:'yyyy-MM-dd' }}</td>
          <td>{{ item.endDate ? (item.endDate | date:'yyyy-MM-dd') : '-' }}</td>
          <td>
            <span [class.active]="item.isActive">
              {{ item.isActive ? 'Yes' : 'No' }}
            </span>
          </td>
          <td>
            <button (click)="edit(item.id)">Edit</button>
            <button class="delete" (click)="remove(item.id)">Delete</button>
          </td>
        </tr>
      </tbody>
    </table>

    <div *ngIf="!isLoading && !recurringList.length">
      No recurring transactions found.
    </div>
  `,
  styles: [`
    h2 {
      margin-bottom: 20px;
    }

    .actions {
      margin-bottom: 20px;
      display: flex;
      gap: 10px;
    }

    button {
      padding: 6px 12px;
      border-radius: 6px;
      border: none;
      cursor: pointer;
      font-weight: 500;
    }

    button:hover {
      opacity: 0.9;
    }

    .run {
      background: #4caf50;
      color: white;
    }

    .delete {
      background: #f44336;
      color: white;
    }

    table {
      width: 100%;
      border-collapse: collapse;
    }

    th, td {
      padding: 8px;
      border-bottom: 1px solid #ddd;
      text-align: left;
    }

    th {
      background: #f5f5f5;
    }

    .active {
      font-weight: bold;
      color: green;
    }
  `]
})
export class RecurringComponent implements OnInit {

  recurringList: RecurringTransaction[] = [];
  isLoading = false;

  constructor(
    private recurringService: RecurringService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadRecurring();
  }

  // loadRecurring() {
  //   this.loading = true;

  //   this.recurringService.getAll().subscribe({
  //     next: (response) => {
  //       console.log('Recurring transactions loaded:', response);
  //       // this.recurringList = response;
  //       this.loading = false;
  //     },
  //     error: () => {
  //       this.loading = false;
  //     }
  //   });
  // }

  loadRecurring() {
  this.isLoading = true;

  this.recurringService.getAll().subscribe({
    next: (response) => {
      console.log('Recurring transactions loaded:', response);

      this.recurringList = response.items; // ✅ correct

      this.isLoading = false;
    },
    error: () => {
      this.isLoading = false;
    }
  });
}

  goToCreate() {
    this.router.navigate(['recurring/create']);
  }

  edit(id: number) {
    this.router.navigate(['recurring/edit', id]);
  }

  remove(id: number) {
    if (!confirm('Are you sure you want to delete this recurring transaction?')) return;

    this.recurringService.delete(id).subscribe(() => {
      this.loadRecurring();
    });
  }

  runNow() {
    this.recurringService.processDue().subscribe(() => {
      alert('Recurring processing executed.');
    });
  }

  getRecurrenceLabel(type: number): string {
    switch (type) {
      case 0: return 'Daily';
      case 1: return 'Weekly';
      case 2: return 'Monthly';
      default: return 'Unknown';
    }
  }
}