import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CategoryService } from '../../core/services/category.service';
import { Category } from '../../core/models/category.model';
import { PagedResponse } from '../../core/models/paged-response.model';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner.component';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule,LoadingSpinnerComponent],
  template: `
    <div class="header">
      <h2>Categories</h2>
      <button class="btn" (click)="goToCreate()">Add Category</button>
    </div>

    <app-loading-spinner *ngIf="isLoading"></app-loading-spinner>

    <div *ngIf="!isLoading && !categories.length">
      No categories found.
    </div>

    <table *ngIf="!isLoading && categories.length">
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
            <button class="btn" (click)="edit(c.id)">Edit</button>
            <button class="btn btn-danger" (click)="remove(c.id)">Delete</button>
          </td>
        </tr>
      </tbody>
    </table>
  `
})
export class CategoriesComponent implements OnInit {

  categories: Category[] = [];
  isLoading = false;

  constructor(
    private categoryService: CategoryService,
    private router: Router
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
    if (!confirm('Delete this category?')) return;

    this.categoryService.delete(id)
      .subscribe(() => this.loadCategories());
  }
}