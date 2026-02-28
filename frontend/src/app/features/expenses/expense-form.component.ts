import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ExpenseService } from '../../core/services/expense.service';
import { CategoryService } from '../../core/services/category.service';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Category } from '../../core/models/category.model';

@Component({
  selector: 'app-expense-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <h2>{{ isEdit ? 'Edit' : 'Create' }} Expense</h2>

    <form [formGroup]="form" (ngSubmit)="submit()">

      <div>
        <label>Description</label>
        <input formControlName="description" />
      </div>

      <div>
        <label>Amount</label>
        <input formControlName="amount" type="number" />
      </div>

      <div>
        <label>Date</label>
        <input formControlName="date" type="date" />
      </div>

      <div>
        <label>Category</label>
        <select formControlName="categoryId">
          <option value="">Select Category</option>
          <option *ngFor="let cat of categories" [value]="cat.id">
            {{ cat.name }}
          </option>
        </select>
      </div>

      <button type="submit" [disabled]="form.invalid">
        Save
      </button>
    </form>
  `
})
export class ExpenseFormComponent implements OnInit {

  form!: FormGroup;
  isEdit = false;
  expenseId!: number;

  categories: Category[] = [];

  constructor(
    private fb: FormBuilder,
    private expenseService: ExpenseService,
    private categoryService: CategoryService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  /**
   * Initialize form and load categories
   */
  ngOnInit(): void {

    this.form = this.fb.group({
      description: ['', Validators.required],
      amount: [0, Validators.required],
      date: ['', Validators.required],
      categoryId: ['', Validators.required]
    });

    // Load categories for dropdown
    this.loadCategories();

    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      this.isEdit = true;
      this.expenseId = +id;

      this.expenseService.getById(this.expenseId)
        .subscribe(exp => {
          this.form.patchValue(exp);
        });
    }
  }

  /**
   * Load all categories
   */
  loadCategories() {
    this.categoryService.getAll()
      .subscribe(data => {
        this.categories = data;
      });
  }

  /**
   * Submit form (Create or Update)
   */
  submit() {

    if (this.form.invalid) return;

    if (this.isEdit) {
      this.expenseService
        .update(this.expenseId, this.form.value)
        .subscribe(() => {
          this.router.navigate(['/expense']);
        });

    } else {
      this.expenseService
        .create(this.form.value)
        .subscribe(() => {
          this.router.navigate(['/expense']);
        });
    }
  }
}