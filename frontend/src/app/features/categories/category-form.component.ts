import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CreateCategoryDto } from '../../core/models/category-create.model';
import { CategoryService } from '../../core/services/category.service';
import { NotificationService } from '../../shared/services/notification.service';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="form-container">
      <h2>{{ isEdit ? 'Edit Category' : 'Create Category' }}</h2>

      <form [formGroup]="form" (ngSubmit)="submit()">
        <div class="form-group">
          <label>Name</label>
          <input type="text" formControlName="name">
          <div class="error" *ngIf="form.get('name')?.touched && form.get('name')?.invalid">
            <small *ngIf="form.get('name')?.errors?.['required']">Category name is required.</small>
            <small *ngIf="form.get('name')?.errors?.['maxlength']">Category name must be 50 characters or fewer.</small>
          </div>
        </div>

        <div class="form-group">
          <label>Description</label>
          <input type="text" formControlName="description">
          <div class="error" *ngIf="form.get('description')?.touched && form.get('description')?.invalid">
            <small *ngIf="form.get('description')?.errors?.['maxlength']">Description must be 120 characters or fewer.</small>
          </div>
        </div>

        <div class="form-group dropdown-container">
          <label>Color</label>
          <div class="color-options">
            <button
              *ngFor="let color of colors"
              type="button"
              class="color-box"
              [style.background]="color"
              [class.selected]="form.get('color')?.value === color"
              [attr.aria-label]="'Select color ' + color"
              (click)="setColor(color)">
            </button>
          </div>

          <div class="selected-color-row">
            <span class="selected-color-label">Selected:</span>
            <span class="selected-color-preview" [style.background]="form.get('color')?.value"></span>
            <span class="selected-color-value">{{ form.get('color')?.value }}</span>
          </div>
        </div>

        <div class="form-actions">
          <button class="btn btn-primary" type="submit" [disabled]="form.invalid">Save</button>
          <button class="btn btn-secondary" type="button" (click)="cancel()">Cancel</button>
        </div>
      </form>
    </div>
  `,
  styles: [`
    .form-container {
      max-width: 500px;
      background: white;
      padding: 25px;
      border-radius: 10px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.08);
    }

    .form-group {
      display: flex;
      flex-direction: column;
      margin-bottom: 15px;
    }

    .form-group label {
      margin-bottom: 5px;
      font-weight: 500;
    }

    .form-group input {
      padding: 8px 10px;
      border: 1px solid #cbd5f5;
      border-radius: 6px;
      background: white;
      font-size: 14px;
      width: 100%;
      max-width: 100%;
      box-sizing: border-box;
    }

    .form-group input:focus {
      outline: none;
      border-color: #6366f1;
    }

    .dropdown-container {
      width: 100%;
      overflow: hidden;
    }

    .color-options {
      display: flex;
      gap: 8px;
      flex-wrap: wrap;
      width: 100%;
      max-width: 100%;
    }

    .color-box {
      width: 24px;
      height: 24px;
      border-radius: 4px;
      cursor: pointer;
      border: 2px solid transparent;
      padding: 0;
      flex: 0 0 auto;
    }

    .color-box.selected {
      border: 2px solid #1e293b;
      box-shadow: 0 0 0 2px rgba(30, 41, 59, 0.12);
    }

    .selected-color-row {
      display: flex;
      align-items: center;
      gap: 8px;
      margin-top: 10px;
      color: #475467;
      font-size: 14px;
      flex-wrap: wrap;
    }

    .selected-color-preview {
      width: 18px;
      height: 18px;
      border-radius: 4px;
      border: 1px solid #d0d5dd;
      display: inline-block;
    }

    .selected-color-value {
      font-family: monospace;
    }

    .form-actions {
      display: flex;
      gap: 10px;
      margin-top: 10px;
    }

    .error {
      margin-top: 6px;
      color: #b42318;
      min-height: 18px;
    }

    .error small {
      display: block;
    }

    @media (max-width: 768px) {
      .form-container {
        max-width: 100%;
        box-sizing: border-box;
      }
    }
  `]
})
export class CategoryFormComponent implements OnInit {
  readonly colors = [
    '#ef4444',
    '#f97316',
    '#eab308',
    '#22c55e',
    '#14b8a6',
    '#3b82f6',
    '#6366f1',
    '#a855f7',
    '#ec4899',
    '#6b7280',
    '#111827',
    '#000000'
  ];

  form!: FormGroup;
  isEdit = false;
  categoryId!: number;

  constructor(
    private fb: FormBuilder,
    private categoryService: CategoryService,
    private route: ActivatedRoute,
    private router: Router,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(50)]],
      description: ['', Validators.maxLength(120)],
      color: ['#000000']
    });

    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      this.isEdit = true;
      this.categoryId = +id;

      this.categoryService.getById(this.categoryId)
        .subscribe(category => {
          this.form.patchValue(category);
        });
    }
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload: CreateCategoryDto = this.form.value;

    if (this.isEdit) {
      this.categoryService.update(this.categoryId, payload)
        .subscribe(() => {
          this.notificationService.success('Category updated successfully.');
          this.router.navigate(['/category']);
        });
      return;
    }

    this.categoryService.create(payload)
      .subscribe(() => {
        this.notificationService.success('Category created successfully.');
        this.router.navigate(['/category']);
      });
  }

  cancel(): void {
    this.router.navigate(['/category']);
  }

  setColor(color: string): void {
    this.form.patchValue({ color });
  }
}
