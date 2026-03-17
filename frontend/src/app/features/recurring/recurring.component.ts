import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { RecurringService } from '../../core/services/recurring.service';
import { RecurringTransaction } from '../../core/models/recurring.model';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner.component';
import { NotificationService } from '../../shared/services/notification.service';
import { ConfirmationService } from '../../shared/services/confirmation.service';

@Component({
  selector: 'app-recurring',
  standalone: true,
  imports: [CommonModule,LoadingSpinnerComponent],
  template: `
    <h2>Recurring Transactions</h2>

    <div class="actions">
      <button class="btn btn-primary" (click)="goToCreate()">Add Recurring</button>
      <button class="btn run" (click)="runNow()">Run Due Now</button>
    </div>

    <app-loading-spinner *ngIf="isLoading"></app-loading-spinner>

    <div class="card" *ngIf="!isLoading && recurringList.length">
    <table class="table" *ngIf="!isLoading && recurringList.length">
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
    </div>

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
    private router: Router,
    private notificationService: NotificationService,
    private confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.loadRecurring();
  }

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
    this.confirmationService.confirm({
      title: 'Delete Recurring Transaction',
      message: 'Are you sure you want to delete this recurring transaction? This action cannot be undone.',
      confirmText: 'Delete',
      cancelText: 'Cancel'
    }).subscribe(confirmed => {
      if (!confirmed) {
        return;
      }

      this.recurringService.delete(id).subscribe(() => {
        this.notificationService.success('Recurring transaction deleted successfully.');
        this.loadRecurring();
      });
    });
  }

  runNow() {
    this.recurringService.processDue().subscribe(() => {
      this.notificationService.success('Recurring processing executed.');
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
