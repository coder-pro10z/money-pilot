import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { Category } from '../../../core/models/category.model';
import { CategoryService } from '../../../core/services/category.service';
import { NotificationService } from '../../services/notification.service';
import { CreateCategoryDto } from '../../../core/models/category-create.model';

@Component({
  selector: 'app-quick-category-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>Create Category</h2>

    <div mat-dialog-content>
      <form [formGroup]="form" class="dialog-form">
        <div class="form-group">
          <label for="category-name">Name</label>
          <input id="category-name" type="text" formControlName="name" />
          <div class="error" *ngIf="form.get('name')?.touched && form.get('name')?.invalid">
            <small *ngIf="form.get('name')?.errors?.['required']">Category name is required.</small>
            <small *ngIf="form.get('name')?.errors?.['maxlength']">Category name must be 50 characters or fewer.</small>
          </div>
        </div>

        <div class="form-group">
          <label for="category-description">Description</label>
          <input id="category-description" type="text" formControlName="description" />
          <div class="error" *ngIf="form.get('description')?.touched && form.get('description')?.invalid">
            <small *ngIf="form.get('description')?.errors?.['maxlength']">Description must be 120 characters or fewer.</small>
          </div>
        </div>

        <div class="form-group">
          <label for="category-color">Color</label>
          <input id="category-color" type="color" formControlName="color" />
        </div>
      </form>
    </div>

    <div mat-dialog-actions align="end">
      <button mat-button type="button" (click)="dialogRef.close()">Cancel</button>
      <button mat-raised-button color="primary" type="button" (click)="save()" [disabled]="isSaving">
        {{ isSaving ? 'Saving...' : 'Create Category' }}
      </button>
    </div>
  `,
  styles: [`
    .dialog-form {
      padding-top: 8px;
    }

    .form-group {
      display: flex;
      flex-direction: column;
      gap: 6px;
      margin-bottom: 16px;
    }

    input {
      padding: 8px 10px;
      border: 1px solid #d0d5dd;
      border-radius: 8px;
    }

    .error {
      color: #b42318;
      min-height: 18px;
    }

    .error small {
      display: block;
    }
  `]
})
export class QuickCategoryDialogComponent {
  isSaving = false;

  readonly form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(50)]],
    description: ['', Validators.maxLength(120)],
    color: ['#3b82f6', Validators.required]
  });

  constructor(
    private fb: FormBuilder,
    private categoryService: CategoryService,
    private notificationService: NotificationService,
    public dialogRef: MatDialogRef<QuickCategoryDialogComponent>
  ) {}

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving = true;

    const payload = this.form.getRawValue() as CreateCategoryDto;

    this.categoryService.create(payload).subscribe({
      next: (createdCategory: Category) => {
        this.notificationService.success('Category created successfully.');
        this.dialogRef.close(createdCategory);
      },
      error: () => {
        this.isSaving = false;
      }
    });
  }
}
