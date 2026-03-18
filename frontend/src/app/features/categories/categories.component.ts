import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Category } from '../../core/models/category.model';
import { CategoryService } from '../../core/services/category.service';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner.component';
import { ConfirmationService } from '../../shared/services/confirmation.service';
import { NotificationService } from '../../shared/services/notification.service';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule, FormsModule, LoadingSpinnerComponent],
  template: `
    <div class="header">
      <h2>Categories</h2>
      <button class="btn btn-primary" (click)="goToCreate()">Add Category</button>
    </div>

    <div class="toolbar" *ngIf="categories.length">
      <input [(ngModel)]="searchTerm" (ngModelChange)="resetPagination()" type="text" placeholder="Search categories" />
      <select [(ngModel)]="sortBy" (ngModelChange)="resetPagination()">
        <option value="name-asc">Name A-Z</option>
        <option value="name-desc">Name Z-A</option>
      </select>
    </div>

    <app-loading-spinner *ngIf="isLoading"></app-loading-spinner>

    <div class="empty-state" *ngIf="!isLoading && !categories.length">
      <h3>No categories yet</h3>
      <p>Create categories to organize expenses, budgets, and recurring transactions.</p>
      <button class="btn btn-primary" (click)="goToCreate()">Create Your First Category</button>
    </div>

    <div class="empty-state" *ngIf="!isLoading && categories.length && !filteredCategories.length">
      <h3>No matching categories</h3>
      <p>Try another search term or clear the filters.</p>
      <button class="btn" (click)="clearFilters()">Clear Filters</button>
    </div>

    <div class="card" *ngIf="filteredCategories.length">
      <table class="data-table">
        <thead>
          <tr>
            <th>Name</th>
            <th>Description</th>
            <th>Color</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let c of pagedCategories">
            <td>{{ c.name }}</td>
            <td>{{ c.description }}</td>
            <td>
              <span
                *ngIf="c.color"
                [style.background]="c.color"
                class="color-chip">
              </span>
            </td>
            <td>
              <div class="actions">
                <button class="btn" (click)="edit(c.id)">Edit</button>
                <button class="btn btn-danger" (click)="remove(c.id)">Delete</button>
              </div>
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

    .actions {
      display: flex;
      gap: 8px;
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

    .color-chip {
      display: inline-block;
      width: 20px;
      height: 20px;
      border-radius: 4px;
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
export class CategoriesComponent implements OnInit {
  categories: Category[] = [];
  isLoading = false;
  searchTerm = '';
  sortBy = 'name-asc';
  currentPage = 1;
  readonly pageSize = 6;

  constructor(
    private categoryService: CategoryService,
    private router: Router,
    private confirmationService: ConfirmationService,
    private notificationService: NotificationService
  ) {}

  get filteredCategories(): Category[] {
    const term = this.searchTerm.trim().toLowerCase();
    const items = this.categories.filter(category =>
      !term ||
      category.name.toLowerCase().includes(term) ||
      category.description?.toLowerCase().includes(term)
    );

    return items.sort((a, b) =>
      this.sortBy === 'name-desc'
        ? b.name.localeCompare(a.name)
        : a.name.localeCompare(b.name)
    );
  }

  get pagedCategories(): Category[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.filteredCategories.slice(start, start + this.pageSize);
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.filteredCategories.length / this.pageSize));
  }

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.isLoading = true;
    this.categoryService.getAll().subscribe(response => {
      this.categories = response;
      this.currentPage = 1;
      this.isLoading = false;
    });
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.sortBy = 'name-asc';
    this.currentPage = 1;
  }

  resetPagination(): void {
    this.currentPage = 1;
  }

  goToCreate(): void {
    this.router.navigate(['/category/create']);
  }

  edit(id: number): void {
    this.router.navigate(['/category/edit', id]);
  }

  previousPage(): void {
    this.currentPage = Math.max(1, this.currentPage - 1);
  }

  nextPage(): void {
    this.currentPage = Math.min(this.totalPages, this.currentPage + 1);
  }

  remove(id: number): void {
    this.confirmationService.confirm({
      title: 'Delete Category',
      message: 'Are you sure you want to delete this category? This action cannot be undone.',
      warningText: 'Deleting this category will also delete all associated expenses and budgets.',
      confirmText: 'Delete',
      cancelText: 'Cancel'
    }).subscribe(confirmed => {
      if (!confirmed) {
        return;
      }

      this.categoryService.delete(id).subscribe(() => {
        this.notificationService.success('Category deleted successfully.');
        this.loadCategories();
      });
    });
  }
}
