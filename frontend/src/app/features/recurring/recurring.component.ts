import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RecurringTransaction } from '../../core/models/recurring.model';
import { RecurringService } from '../../core/services/recurring.service';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner.component';
import { NotificationService } from '../../shared/services/notification.service';
import { ConfirmationService } from '../../shared/services/confirmation.service';

@Component({
  selector: 'app-recurring',
  standalone: true,
  imports: [CommonModule, FormsModule, LoadingSpinnerComponent],
  template: `
    <h2>Recurring Transactions</h2>

    <div class="actions">
      <button class="btn btn-primary" (click)="goToCreate()">Add Recurring</button>
      <button class="btn run" (click)="runNow()">Run Due Now</button>
    </div>

    <div class="toolbar" *ngIf="recurringList.length">
      <input [(ngModel)]="searchTerm" (ngModelChange)="resetPagination()" type="text" placeholder="Search description or category" />

      <select [(ngModel)]="statusFilter" (ngModelChange)="resetPagination()">
        <option value="all">All Statuses</option>
        <option value="active">Active Only</option>
        <option value="inactive">Inactive Only</option>
      </select>

      <select [(ngModel)]="typeFilter" (ngModelChange)="resetPagination()">
        <option value="all">All Types</option>
        <option *ngFor="let type of recurrenceTypes" [value]="type">{{ type }}</option>
      </select>
    </div>

    <app-loading-spinner *ngIf="isLoading"></app-loading-spinner>

    <div class="card" *ngIf="filteredRecurring.length">
      <table class="table data-table">
        <thead>
          <tr>
            <th>Description</th>
            <th>Amount</th>
            <th>Category</th>
            <th>Type</th>
            <th>Schedule</th>
            <th>Next</th>
            <th>Active</th>
            <th>Actions</th>
          </tr>
        </thead>

        <tbody>
          <tr *ngFor="let item of pagedRecurring">
            <td>{{ item.description }}</td>
            <td>{{ item.amount | currency }}</td>
            <td>{{ item.categoryName }}</td>
            <td>{{ getRecurrenceLabel(item.recurrenceType) }}</td>
            <td>{{ getScheduleSummary(item) }}</td>
            <td>{{ item.nextOccurrence ? (item.nextOccurrence | date:'mediumDate') : '-' }}</td>
            <td>
              <span [class.active]="item.isActive">{{ item.isActive ? 'Yes' : 'No' }}</span>
            </td>
            <td>
              <button (click)="edit(item.id)">Edit</button>
              <button class="delete" (click)="remove(item.id)">Delete</button>
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

    <div class="empty-state" *ngIf="!isLoading && !recurringList.length">
      <h3>No recurring transactions yet</h3>
      <p>Set up recurring items for rent, subscriptions, and other repeat spending.</p>
      <button class="btn btn-primary" (click)="goToCreate()">Add Your First Recurring Transaction</button>
    </div>

    <div class="empty-state" *ngIf="!isLoading && recurringList.length && !filteredRecurring.length">
      <h3>No matching recurring transactions</h3>
      <p>Adjust the filters or clear them to see all schedules again.</p>
      <button class="btn" (click)="clearFilters()">Clear Filters</button>
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
export class RecurringComponent implements OnInit {
  recurringList: RecurringTransaction[] = [];
  isLoading = false;
  searchTerm = '';
  statusFilter = 'all';
  typeFilter = 'all';
  currentPage = 1;
  readonly pageSize = 5;
  readonly recurrenceTypes = ['Daily', 'Weekly', 'Monthly', 'Yearly'];

  constructor(
    private recurringService: RecurringService,
    private router: Router,
    private notificationService: NotificationService,
    private confirmationService: ConfirmationService
  ) {}

  get filteredRecurring(): RecurringTransaction[] {
    const term = this.searchTerm.trim().toLowerCase();

    return this.recurringList.filter(item => {
      const type = this.getRecurrenceLabel(item.recurrenceType);
      const matchesSearch =
        !term ||
        item.description.toLowerCase().includes(term) ||
        item.categoryName?.toLowerCase().includes(term);

      const matchesStatus =
        this.statusFilter === 'all' ||
        (this.statusFilter === 'active' && item.isActive) ||
        (this.statusFilter === 'inactive' && !item.isActive);

      const matchesType = this.typeFilter === 'all' || type === this.typeFilter;

      return matchesSearch && matchesStatus && matchesType;
    });
  }

  get pagedRecurring(): RecurringTransaction[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.filteredRecurring.slice(start, start + this.pageSize);
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.filteredRecurring.length / this.pageSize));
  }

  ngOnInit(): void {
    this.loadRecurring();
  }

  loadRecurring(): void {
    this.isLoading = true;

    this.recurringService.getAll().subscribe({
      next: (response) => {
        this.recurringList = response.items;
        this.currentPage = 1;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.statusFilter = 'all';
    this.typeFilter = 'all';
    this.currentPage = 1;
  }

  resetPagination(): void {
    this.currentPage = 1;
  }

  goToCreate(): void {
    this.router.navigate(['recurring/create']);
  }

  edit(id: number): void {
    this.router.navigate(['recurring/edit', id]);
  }

  previousPage(): void {
    this.currentPage = Math.max(1, this.currentPage - 1);
  }

  nextPage(): void {
    this.currentPage = Math.min(this.totalPages, this.currentPage + 1);
  }

  remove(id: number): void {
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

  runNow(): void {
    this.recurringService.processDue().subscribe(() => {
      this.notificationService.success('Recurring processing executed.');
    });
  }

  getRecurrenceLabel(type: string | number): string {
    const map: Record<string, string> = {
      '1': 'Daily',
      '2': 'Weekly',
      '3': 'Monthly',
      '4': 'Yearly',
      Daily: 'Daily',
      Weekly: 'Weekly',
      Monthly: 'Monthly',
      Yearly: 'Yearly'
    };

    return map[String(type)] ?? 'Unknown';
  }

  getScheduleSummary(item: RecurringTransaction): string {
    const interval = item.interval ?? 1;
    const type = this.getRecurrenceLabel(item.recurrenceType);

    if (type === 'Weekly') {
      return `Every ${interval} week(s) on ${item.dayOfWeek || '-'}`;
    }

    if (type === 'Monthly' || type === 'Yearly') {
      return `Every ${interval} ${type.toLowerCase()}(s) on day ${item.dayOfMonth || '-'}`;
    }

    return `Every ${interval} day(s)`;
  }
}
