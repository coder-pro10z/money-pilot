import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CategoryService } from '../../core/services/category.service';
import { RecurringTransaction } from '../../core/models/recurring.model';
import { RecurringService } from '../../core/services/recurring.service';
import { NotificationService } from '../../shared/services/notification.service';

type RecurrenceOption = 'Daily' | 'Weekly' | 'Monthly' | 'Yearly';

@Component({
  selector: 'app-recurring-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="form-container">
      <h2>{{ isEdit ? 'Edit Recurring Transaction' : 'Create Recurring Transaction' }}</h2>

      <form [formGroup]="form" (ngSubmit)="submit()">
        <div class="form-group">
          <label>Description</label>
          <input type="text" formControlName="description" />
          <div class="error" *ngIf="form.get('description')?.touched && form.get('description')?.invalid">
            <small *ngIf="form.get('description')?.errors?.['required']">Description is required.</small>
            <small *ngIf="form.get('description')?.errors?.['maxlength']">Description must be 100 characters or fewer.</small>
          </div>
        </div>

        <div class="form-grid">
          <div class="form-group">
            <label>Amount</label>
            <input type="number" formControlName="amount" />
            <div class="error" *ngIf="form.get('amount')?.touched && form.get('amount')?.invalid">
              <small *ngIf="form.get('amount')?.errors?.['required']">Amount is required.</small>
              <small *ngIf="form.get('amount')?.errors?.['min']">Amount must be greater than 0.</small>
            </div>
          </div>

          <div class="form-group">
            <label>Category</label>
            <select formControlName="categoryId">
              <option value="">Select Category</option>
              <option *ngFor="let c of categories" [value]="c.id">{{ c.name }}</option>
            </select>
            <div class="error" *ngIf="form.get('categoryId')?.touched && form.get('categoryId')?.invalid">
              <small *ngIf="form.get('categoryId')?.errors?.['required']">Please select a category.</small>
            </div>
          </div>
        </div>

        <div class="form-grid">
          <div class="form-group">
            <label>Recurrence Type</label>
            <select formControlName="recurrenceType">
              <option *ngFor="let type of recurrenceOptions" [value]="type">{{ type }}</option>
            </select>
          </div>

          <div class="form-group">
            <label>Repeat Every</label>
            <input type="number" formControlName="interval" min="1" />
            <div class="hint">{{ recurrenceUnitHint }}</div>
            <div class="error" *ngIf="form.get('interval')?.touched && form.get('interval')?.invalid">
              <small *ngIf="form.get('interval')?.errors?.['required']">Interval is required.</small>
              <small *ngIf="form.get('interval')?.errors?.['min']">Interval must be at least 1.</small>
            </div>
          </div>
        </div>

        <div class="form-grid" *ngIf="isWeekly">
          <div class="form-group">
            <label>Day Of Week</label>
            <select formControlName="dayOfWeek">
              <option value="">Select Day</option>
              <option *ngFor="let day of weekDays" [value]="day">{{ day }}</option>
            </select>
            <div class="error" *ngIf="form.get('dayOfWeek')?.touched && form.get('dayOfWeek')?.invalid">
              <small *ngIf="form.get('dayOfWeek')?.errors?.['required']">Please select a weekday.</small>
            </div>
          </div>
        </div>

        <div class="form-grid" *ngIf="requiresDayOfMonth">
          <div class="form-group">
            <label>Day Of Month</label>
            <input type="number" formControlName="dayOfMonth" min="1" max="31" />
            <div class="hint" *ngIf="isYearly">The month will follow the start date.</div>
            <div class="error" *ngIf="form.get('dayOfMonth')?.touched && form.get('dayOfMonth')?.invalid">
              <small *ngIf="form.get('dayOfMonth')?.errors?.['required']">Day of month is required.</small>
              <small *ngIf="form.get('dayOfMonth')?.errors?.['min'] || form.get('dayOfMonth')?.errors?.['max']">
                Day of month must be between 1 and 31.
              </small>
            </div>
          </div>
        </div>

        <div class="form-grid">
          <div class="form-group">
            <label>Start Date</label>
            <input type="date" formControlName="startDate" />
            <div class="error" *ngIf="form.get('startDate')?.touched && form.get('startDate')?.invalid">
              <small *ngIf="form.get('startDate')?.errors?.['required']">Start date is required.</small>
            </div>
          </div>

          <div class="form-group">
            <label>End Date (Optional)</label>
            <input type="date" formControlName="endDate" />
          </div>
        </div>

        <div class="schedule-preview">
          <strong>Schedule Preview:</strong> {{ schedulePreview }}
        </div>

        <div class="form-group checkbox">
          <label>
            <input type="checkbox" formControlName="isActive" />
            Active
          </label>
        </div>

        <div class="form-actions">
          <button type="submit" [disabled]="form.invalid">Save</button>
          <button type="button" class="cancel" (click)="router.navigate(['/recurring'])">Cancel</button>
        </div>
      </form>
    </div>
  `,
  styles: [`
    .form-container {
      max-width: 680px;
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

    .form-grid {
      display: grid;
      grid-template-columns: repeat(2, minmax(0, 1fr));
      gap: 16px;
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

    .hint {
      margin-top: 6px;
      color: #475467;
      font-size: 12px;
    }

    .schedule-preview {
      margin: 8px 0 16px;
      padding: 12px 14px;
      background: #f8fafc;
      border: 1px solid #d0d5dd;
      border-radius: 8px;
      color: #344054;
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

    @media (max-width: 768px) {
      .form-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class RecurringFormComponent implements OnInit {
  readonly recurrenceOptions: RecurrenceOption[] = ['Daily', 'Weekly', 'Monthly', 'Yearly'];
  readonly weekDays = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

  isEdit = false;
  recurringId!: number;
  categories: any[] = [];

  readonly form = this.fb.group({
    description: ['', [Validators.required, Validators.maxLength(100)]],
    amount: [null as number | null, [Validators.required, Validators.min(0.01)]],
    categoryId: [null as number | null, Validators.required],
    recurrenceType: ['Monthly' as RecurrenceOption, Validators.required],
    interval: [1, [Validators.required, Validators.min(1)]],
    dayOfWeek: [''],
    dayOfMonth: [null as number | null],
    startDate: ['', Validators.required],
    endDate: [''],
    isActive: [true]
  });

  constructor(
    private fb: FormBuilder,
    private recurringService: RecurringService,
    private categoryService: CategoryService,
    private route: ActivatedRoute,
    public router: Router,
    private notificationService: NotificationService
  ) {}

  get selectedType(): RecurrenceOption {
    return this.form.get('recurrenceType')?.value as RecurrenceOption;
  }

  get isWeekly(): boolean {
    return this.selectedType === 'Weekly';
  }

  get isYearly(): boolean {
    return this.selectedType === 'Yearly';
  }

  get requiresDayOfMonth(): boolean {
    return this.selectedType === 'Monthly' || this.selectedType === 'Yearly';
  }

  get recurrenceUnitHint(): string {
    switch (this.selectedType) {
      case 'Daily':
        return 'For example: every 1 day or every 3 days.';
      case 'Weekly':
        return 'For example: every 1 week or every 2 weeks.';
      case 'Monthly':
        return 'For example: every 1 month or every 2 months.';
      case 'Yearly':
        return 'For example: every 1 year.';
      default:
        return '';
    }
  }

  get schedulePreview(): string {
    const interval = this.form.get('interval')?.value ?? 1;
    const startDate = this.form.get('startDate')?.value || 'your chosen start date';

    switch (this.selectedType) {
      case 'Daily':
        return `Repeats every ${interval} day(s), starting ${startDate}.`;
      case 'Weekly':
        return `Repeats every ${interval} week(s) on ${this.form.get('dayOfWeek')?.value || 'a selected weekday'}, starting ${startDate}.`;
      case 'Monthly':
        return `Repeats every ${interval} month(s) on day ${this.form.get('dayOfMonth')?.value || '--'}, starting ${startDate}.`;
      case 'Yearly':
        return `Repeats every ${interval} year(s) on day ${this.form.get('dayOfMonth')?.value || '--'} of the start month, starting ${startDate}.`;
      default:
        return 'Configure your schedule.';
    }
  }

  ngOnInit(): void {
    this.loadCategories();
    this.handleRecurrenceRules();

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit = true;
      this.recurringId = +id;
      this.loadRecurringTransaction(this.recurringId);
    }
  }

  private loadCategories(): void {
    this.categoryService.getAll().subscribe(res => {
      this.categories = res;
    });
  }

  private handleRecurrenceRules(): void {
    this.form.get('recurrenceType')?.valueChanges.subscribe(type => {
      const dayOfWeekControl = this.form.get('dayOfWeek');
      const dayOfMonthControl = this.form.get('dayOfMonth');

      dayOfWeekControl?.clearValidators();
      dayOfMonthControl?.clearValidators();

      if (type === 'Weekly') {
        dayOfWeekControl?.setValidators([Validators.required]);
        dayOfMonthControl?.setValue(null);
      }

      if (type === 'Monthly' || type === 'Yearly') {
        dayOfMonthControl?.setValidators([Validators.required, Validators.min(1), Validators.max(31)]);
        dayOfWeekControl?.setValue('');
      }

      if (type === 'Daily') {
        dayOfWeekControl?.setValue('');
        dayOfMonthControl?.setValue(null);
      }

      dayOfWeekControl?.updateValueAndValidity();
      dayOfMonthControl?.updateValueAndValidity();
    });

    this.form.get('recurrenceType')?.updateValueAndValidity({ emitEvent: true });
  }

  private loadRecurringTransaction(id: number): void {
    this.recurringService.getById(id).subscribe(data => {
      const normalized = this.normalizeRecurring(data);

      this.form.patchValue({
        description: normalized.description,
        amount: normalized.amount,
        categoryId: normalized.categoryId,
        recurrenceType: normalized.recurrenceType,
        interval: normalized.interval ?? 1,
        dayOfWeek: normalized.dayOfWeek ?? '',
        dayOfMonth: normalized.dayOfMonth ?? null,
        startDate: normalized.startDate?.substring(0, 10),
        endDate: normalized.endDate ? normalized.endDate.substring(0, 10) : '',
        isActive: normalized.isActive
      });
    });
  }

  private normalizeRecurring(data: RecurringTransaction): RecurringTransaction & { recurrenceType: RecurrenceOption } {
    const recurrenceMap: Record<string, RecurrenceOption> = {
      '1': 'Daily',
      '2': 'Weekly',
      '3': 'Monthly',
      '4': 'Yearly',
      Daily: 'Daily',
      Weekly: 'Weekly',
      Monthly: 'Monthly',
      Yearly: 'Yearly'
    };

    const normalizedType = recurrenceMap[String(data.recurrenceType)] ?? 'Monthly';

    return {
      ...data,
      recurrenceType: normalizedType
    };
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    const payload = {
      description: raw.description ?? '',
      amount: Number(raw.amount),
      categoryId: Number(raw.categoryId),
      recurrenceType: raw.recurrenceType,
      interval: Number(raw.interval ?? 1),
      dayOfWeek: this.isWeekly ? raw.dayOfWeek || null : null,
      dayOfMonth: this.requiresDayOfMonth ? Number(raw.dayOfMonth) : null,
      startDate: new Date(raw.startDate || '').toISOString(),
      endDate: raw.endDate ? new Date(raw.endDate).toISOString() : null,
      isActive: raw.isActive
    };

    if (this.isEdit) {
      this.recurringService.update(this.recurringId, payload).subscribe(() => {
        this.notificationService.success('Recurring transaction updated successfully.');
        this.router.navigate(['/recurring']);
      });
      return;
    }

    this.recurringService.create(payload).subscribe(() => {
      this.notificationService.success('Recurring transaction created successfully.');
      this.router.navigate(['/recurring']);
    });
  }
}
