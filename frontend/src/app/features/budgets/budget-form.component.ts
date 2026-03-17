import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ExpenseService } from '../../core/services/expense.service';
import { CategoryService } from '../../core/services/category.service';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { BudgetService } from '../../core/services/budget.service';
import { CreateBudgetDto } from '../../core/models/budget-create.model';
import { NotificationService } from '../../shared/services/notification.service';

@Component({
  selector: 'app-budget-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="form-container">
      <h2>{{ isEdit ? 'Edit Budget' : 'Create Budget' }}</h2>

      <form [formGroup]="form" (ngSubmit)="submit()">

        <!-- Description 
        <div class="form-group">
          <label>Description</label>
          <input formControlName="description" type="text" />
        </div> -->

        <!-- Monthly Limit -->
        <div class="form-group">
          <label>Monthly Limit</label>
          <input formControlName="monthlyLimit" type="number" />
          <div class="error" *ngIf="form.get('monthlyLimit')?.touched && form.get('monthlyLimit')?.invalid">
            <small *ngIf="form.get('monthlyLimit')?.errors?.['required']">Monthly limit is required.</small>
            <small *ngIf="form.get('monthlyLimit')?.errors?.['min']">Monthly limit must be greater than 0.</small>
          </div>
        </div>

        <!-- Month -->
        <div class="form-group">
          <label>Month</label>
          <input formControlName="month" type="month" />
          <div class="error" *ngIf="form.get('month')?.touched && form.get('month')?.invalid">
            <small *ngIf="form.get('month')?.errors?.['required']">Month is required.</small>
          </div>
        </div>

        <!-- Category -->
        <div class="form-group">
          <label>Category</label>
          <select formControlName="categoryId">
            <option value="">Select Category</option>
            <option *ngFor="let c of categories" [value]="c.id">
              {{ c.name }}
            </option>
          </select>
          <div class="error" *ngIf="form.get('categoryId')?.touched && form.get('categoryId')?.invalid">
            <small *ngIf="form.get('categoryId')?.errors?.['required']">Please select a category.</small>
          </div>
        </div>

        <!-- Actions -->
        <div class="form-actions">
          <button type="submit" [disabled]="form.invalid">
            Save
          </button>

          <button type="button" class="cancel"
                  (click)="router.navigate(['/budget'])">
            Cancel
          </button>
        </div>

      </form>
    </div>
  `,
  styles: [`
    .form-container {
      max-width: 500px;
      margin: 40px auto;
      padding: 30px;
      background: #ffffff;
      border-radius: 8px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.08);
    }

    h2 {
      margin-bottom: 20px;
      font-weight: 600;
      text-align: center;
    }

    .form-group {
      margin-bottom: 18px;
      display: flex;
      flex-direction: column;
    }

    label {
      margin-bottom: 6px;
      font-weight: 500;
      font-size: 14px;
    }

    input, select {
      padding: 8px 10px;
      border-radius: 6px;
      border: 1px solid #ccc;
      font-size: 14px;
      transition: border 0.2s ease;
    }

    input:focus, select:focus {
      outline: none;
      border-color: #3f51b5;
    }

    .form-actions {
      display: flex;
      justify-content: space-between;
      margin-top: 20px;
    }

    button {
      padding: 8px 16px;
      border: none;
      border-radius: 6px;
      cursor: pointer;
      font-weight: 500;
      transition: background 0.2s ease;
    }

    button[type="submit"] {
      background: #3f51b5;
      color: white;
    }

    button[type="submit"]:hover {
      background: #303f9f;
    }

    button[type="submit"]:disabled {
      background: #ccc;
      cursor: not-allowed;
    }

    .cancel {
      background: #e0e0e0;
    }

    .cancel:hover {
      background: #c6c6c6;
    }

    .error {
      margin-top: 6px;
      color: #b42318;
      min-height: 18px;
    }

    .error small {
      display: block;
    }
  `]
})
export class BudgetFormComponent implements OnInit {

  form!: FormGroup;
  isEdit = false;
  BudgetId!: number;
  categories: any[] = [];

  constructor(
    private fb: FormBuilder,
    private budgetService: BudgetService,
    private categoryService: CategoryService,
    private route: ActivatedRoute,
    public router: Router,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {

  // ✅ Correct form structure matching backend DTO
  this.form = this.fb.group({
    monthlyLimit: [0, [Validators.required, Validators.min(1)]],
    month: ['', Validators.required],
    categoryId: [null, Validators.required]
  });

  // ✅ Load categories (paged response handled)
  this.categoryService.getAll().subscribe(response => {
    this.categories =  response;
  });

  // ✅ Detect edit mode
  const id = this.route.snapshot.paramMap.get('id');

  if (id) {
    this.isEdit = true;
    this.BudgetId = +id;

    this.budgetService.getById(this.BudgetId)
      .subscribe(budget => {

        console.log('Budget to edit:', budget);

        this.form.patchValue({
          monthlyLimit: budget.monthlyLimit,
          month: budget.month,
          categoryId: budget.categoryId
        });
      });
  }
}



  submit() {

  if (this.form.invalid) {
    this.form.markAllAsTouched();
    return;
  }

  // ✅ Transform month to ISO date (first day of month)
  const monthValue = this.form.value.month;
  const formattedMonth = new Date(monthValue + '-01').toISOString();

  // ✅ Convert categoryId to number
  const payload: CreateBudgetDto = {
    monthlyLimit: Number(this.form.value.monthlyLimit),
    categoryId: Number(this.form.value.categoryId),
    month: formattedMonth
  };

  console.log('Sending payload:', payload);

  if (this.isEdit) {
    this.budgetService.update(this.BudgetId, payload)
      .subscribe(() => {
        this.notificationService.success('Budget updated successfully.');
        this.router.navigate(['/budget']);
      });
  } else {
    this.budgetService.create(payload)
      .subscribe(() => {
        this.notificationService.success('Budget created successfully.');
        this.router.navigate(['/budget']);
      });
  }
}
}
