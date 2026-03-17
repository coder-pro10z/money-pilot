import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CategoryService } from '../../core/services/category.service';
import { Category } from '../../core/models/category.model';
import { PagedResponse } from '../../core/models/paged-response.model';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner.component';
import { ConfirmationService } from '../../shared/services/confirmation.service';
import { NotificationService } from '../../shared/services/notification.service';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule,LoadingSpinnerComponent],
  template: `
    <div class="header">
      <h2>Categories</h2>
      <button class="btn btn-primary" (click)="goToCreate()">Add Category</button>
    </div>

    <app-loading-spinner *ngIf="isLoading"></app-loading-spinner>

    <div class="empty-state" *ngIf="!isLoading && !categories.length">
      <h3>No categories yet</h3>
      <p>Create categories to organize expenses, budgets, and recurring transactions.</p>
      <button class="btn btn-primary" (click)="goToCreate()">Create Your First Category</button>
    </div>

    <div class="card" *ngIf="!isLoading && categories.length">
    <table  class="data-table" *ngIf="!isLoading && categories.length">
      <thead>
        <tr>
          <th>Name</th>
          <th>Description</th>
          <th>Color</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let c of categories">
          <td>{{ c.name }}</td>
          <td>{{ c.description }}</td>
          <td>
            <span
              *ngIf="c.color"
              [style.background]="c.color"
              style="display:inline-block;width:20px;height:20px;border-radius:4px;">
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
    </div>
  `,
  styles: [`
    .header {
      display: flex;  
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1rem;
    } 
      .actions{
  display:flex;
  gap:8px;
}
  
/* Categories table: Name, Description, Color, Actions */
.data-table th:nth-child(1) { width: 25%; }  /* Name */
.data-table th:nth-child(2) { width: 30%; }  /* Description */
.data-table th:nth-child(3) { width: 10%; }  /* Color */
.data-table th:nth-child(4) { width: 20%; }  /* Actions */

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
`]

})
export class CategoriesComponent implements OnInit {

  categories: Category[] = [];
  isLoading = false;

  constructor(
    private categoryService: CategoryService,
    private router: Router,
    private confirmationService: ConfirmationService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories() {
    this.isLoading = true;

    this.categoryService.getAll()
      .subscribe(response=> {
        this.categories = response;
        this.isLoading = false;
      });
  }


  goToCreate() {
    this.router.navigate(['/category/create']);
  }

  edit(id: number) {
    this.router.navigate(['/category/edit', id]);
  }

  remove(id: number) {
    this.confirmationService.confirm({
      title: 'Delete Category',
      message: 'Are you sure you want to delete this category? This action cannot be undone.',
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
