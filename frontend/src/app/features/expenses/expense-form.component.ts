import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ExpenseService } from '../../core/services/expense.service';
import { CategoryService } from '../../core/services/category.service';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../shared/services/notification.service';

@Component({
  selector: 'app-expense-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="form-container">
      <h2>{{ isEdit ? 'Edit Expense' : 'Create Expense' }}</h2>

      <form [formGroup]="form" (ngSubmit)="submit()">

        <!-- Description -->
        <div class="form-group">
          <label>Description</label>
          <input formControlName="description" type="text" />
          <div class="error" *ngIf="form.get('description')?.touched && form.get('description')?.invalid">
            <small *ngIf="form.get('description')?.errors?.['required']">Description is required.</small>
            <small *ngIf="form.get('description')?.errors?.['maxlength']">Description must be 100 characters or fewer.</small>
          </div>
        </div>

        <!-- Amount -->
        <div class="form-group">
          <label>Amount</label>
          <input formControlName="amount" type="number" />
          <div class="error" *ngIf="form.get('amount')?.touched && form.get('amount')?.invalid">
            <small *ngIf="form.get('amount')?.errors?.['required']">Amount is required.</small>
            <small *ngIf="form.get('amount')?.errors?.['min']">Amount must be greater than 0.</small>
          </div>
        </div>

        <!-- Date -->
        <div class="form-group">
          <label>Date</label>
          <input formControlName="date" type="date" />
          <div class="error" *ngIf="form.get('date')?.touched && form.get('date')?.invalid">
            <small *ngIf="form.get('date')?.errors?.['required']">Date is required.</small>
          </div>
        </div>

        <!-- Category -->
        <div class="form-group">

        <label>Category</label>

        <div class="category-row">

        <select formControlName="categoryId">

        <option value="">Select Category</option>

        <option *ngFor="let c of categories"
                [value]="c.id">
                {{c.name}}
        </option>

        </select>

        <button type="button"
                class="btn btn-secondary"
                (click)="goToAddCategory()">

        + Add Category

        </button>

        </div>

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
                  (click)="router.navigate(['/expense'])">
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
     margin-right: 0.5rem;
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

    .category-row{
      display:flex;
      gap:10px;
      align-items:center;
    }

    .category-row select{
      flex:1;
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
export class ExpenseFormComponent implements OnInit {

  form!: FormGroup;
  isEdit = false;
  expenseId!: number;
  categories: any[] = [];

  constructor(
    private fb: FormBuilder,
    private expenseService: ExpenseService,
    private categoryService: CategoryService,
    private route: ActivatedRoute,
    public router: Router,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {

    this.form = this.fb.group({
      description: ['', [Validators.required, Validators.maxLength(100)]],
      amount: [null, [Validators.required, Validators.min(0.01)]],
      date: ['', Validators.required],
      categoryId: [null, Validators.required]
    });

    // Load categories
    this.categoryService.getAll().subscribe(res => {
      this.categories = res;
    });

    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      this.isEdit = true;
      this.expenseId = +id;

      this.expenseService.getById(this.expenseId)
        .subscribe(exp => {
          this.form.patchValue({
            description: exp.description,
            amount: exp.amount,
            date: exp.date?.substring(0, 10),
            categoryId: exp.categoryId
          });
        });
    }
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    if (this.isEdit) {
      this.expenseService.update(this.expenseId, this.form.value)
        .subscribe(() => {
          this.notificationService.success('Expense updated successfully.');
          this.router.navigate(['/expense']);
        });
    } else {
      this.expenseService.create(this.form.value)
        .subscribe(() => {
          this.notificationService.success('Expense created successfully.');
          this.router.navigate(['/expense']);
        });
    }
  }

  goToAddCategory(){
  this.router.navigate(['/category/create']);
}
}
