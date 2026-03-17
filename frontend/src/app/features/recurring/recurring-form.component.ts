import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RecurringService } from '../../core/services/recurring.service';
import { CategoryService } from '../../core/services/category.service';

@Component({
  selector: 'app-recurring-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="form-container">
      <h2>{{ isEdit ? 'Edit Recurring Transaction' : 'Create Recurring Transaction' }}</h2>

      <form [formGroup]="form" (ngSubmit)="submit()">

        <!-- Description -->
        <div class="form-group">
          <label>Description</label>
          <input type="text" formControlName="description" />
          <div class="error" *ngIf="form.get('description')?.touched && form.get('description')?.invalid">
            <small *ngIf="form.get('description')?.errors?.['required']">Description is required.</small>
            <small *ngIf="form.get('description')?.errors?.['maxlength']">Description must be 100 characters or fewer.</small>
          </div>
        </div>

        <!-- Amount -->
        <div class="form-group">
          <label>Amount</label>
          <input type="number" formControlName="amount" />
          <div class="error" *ngIf="form.get('amount')?.touched && form.get('amount')?.invalid">
            <small *ngIf="form.get('amount')?.errors?.['required']">Amount is required.</small>
            <small *ngIf="form.get('amount')?.errors?.['min']">Amount must be greater than 0.</small>
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

        <!-- Recurrence Type -->
        <div class="form-group">
          <label>Recurrence Type</label>
          <select formControlName="recurrenceType">
            <option [value]="0">Daily</option>
            <option [value]="1">Weekly</option>
            <option [value]="2">Monthly</option>
          </select>
          <div class="error" *ngIf="form.get('recurrenceType')?.touched && form.get('recurrenceType')?.invalid">
            <small *ngIf="form.get('recurrenceType')?.errors?.['required']">Recurrence type is required.</small>
          </div>
        </div>

        <!-- Start Date -->
        <div class="form-group">
          <label>Start Date</label>
          <input type="date" formControlName="startDate" />
          <div class="error" *ngIf="form.get('startDate')?.touched && form.get('startDate')?.invalid">
            <small *ngIf="form.get('startDate')?.errors?.['required']">Start date is required.</small>
          </div>
        </div>

        <!-- End Date -->
        <div class="form-group">
          <label>End Date (Optional)</label>
          <input type="date" formControlName="endDate" />
        </div>

        <!-- Active -->
        <div class="form-group checkbox">
          <label>
            <input type="checkbox" formControlName="isActive" />
            Active
          </label>
        </div>

        <!-- Actions -->
        <div class="form-actions">
          <button type="submit" [disabled]="form.invalid">
            Save
          </button>

          <button type="button" class="cancel" (click)="router.navigate(['/recurring'])">
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
      text-align: center;
    }

    .form-group {
      margin-bottom: 16px;
      display: flex;
      flex-direction: column;
    }

    label {
      margin-bottom: 6px;
      font-weight: 500;
    }

    input, select {
      padding: 8px 10px;
      border-radius: 6px;
      border: 1px solid #ccc;
    }

    input:focus, select:focus {
      outline: none;
      border-color: #3f51b5;
    }

    .checkbox {
      flex-direction: row;
      align-items: center;
    }

    .form-actions {
      display: flex;
      justify-content: space-between;
      margin-top: 20px;
    }

    button {
      padding: 8px 16px;
      border-radius: 6px;
      border: none;
      cursor: pointer;
      font-weight: 500;
    }

    button[type="submit"] {
      background: #3f51b5;
      color: white;
    }

    button[type="submit"]:hover {
      background: #303f9f;
    }

    .cancel {
      background: #e0e0e0;
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
export class RecurringFormComponent implements OnInit {

  form!: FormGroup;
  isEdit = false;
  recurringId!: number;
  categories: any[] = [];

  constructor(
    private fb: FormBuilder,
    private recurringService: RecurringService,
    private categoryService: CategoryService,
    private route: ActivatedRoute,
    public router: Router
  ) {}

  ngOnInit(): void {

    this.form = this.fb.group({
      description: ['', [Validators.required, Validators.maxLength(100)]],
      amount: [null, [Validators.required, Validators.min(0.01)]],
      categoryId: [null, Validators.required],
      recurrenceType: [2, Validators.required], // default Monthly
      startDate: ['', Validators.required],
      endDate: [''],
      isActive: [true]
    });

    // Load categories
    this.categoryService.getAll().subscribe(res => {
      this.categories = res;
    });

    // Edit Mode
    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      this.isEdit = true;
      this.recurringId = +id;

      this.recurringService.getById(this.recurringId)
        .subscribe(data => {
          this.form.patchValue({
            description: data.description,
            amount: data.amount,
            categoryId: data.categoryId,
            recurrenceType: data.recurrenceType,
            startDate: data.startDate?.substring(0, 10),
            endDate: data.endDate ? data.endDate.substring(0, 10) : '',
            isActive: data.isActive
          });
        });
    }
  }

  submit() {

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.value;

    const payload = {
      description: raw.description,
      amount: Number(raw.amount),
      categoryId: Number(raw.categoryId),
      recurrenceType: Number(raw.recurrenceType),
      startDate: new Date(raw.startDate).toISOString(),
      endDate: raw.endDate ? new Date(raw.endDate).toISOString() : null,
      isActive: raw.isActive
    };

    if (this.isEdit) {
      this.recurringService.update(this.recurringId, payload)
        .subscribe(() => this.router.navigate(['/recurring']));
    } else {
      this.recurringService.create(payload)
        .subscribe(() => this.router.navigate(['/recurring']));
    }
  }
}
