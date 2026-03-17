import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CategoryService } from '../../core/services/category.service';
import { CreateCategoryDto } from '../../core/models/category-create.model';
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

      <div class="form-group">
        <label>Color</label>
        <input type="color" formControlName="color">
      </div>

      <div class="form-actions">
        <button class="btn btn-primary" type="submit" [disabled]="form.invalid">Save</button>
        <button class="btn btn-secondary" type="button" (click)="cancel()">Cancel</button>
      </div>

    </form>

  </div>
  `,
  styles:[`
    .form-container{
      max-width:500px;
      background:white;
      padding:25px;
      border-radius:10px;
      box-shadow:0 2px 8px rgba(0,0,0,0.08);
    }

    .form-group{
      display:flex;
      flex-direction:column;
      margin-bottom:15px;
    }

    .form-group label{
      margin-bottom:5px;
      font-weight:500;
    }

    .form-group input{
      padding:8px;
      border:1px solid #ddd;
      border-radius:6px;
    }

    .form-actions{
      display:flex;
      gap:10px;
      margin-top:10px;
    }

    .error{
      margin-top:6px;
      color:#b42318;
      min-height:18px;
    }

    .error small{
      display:block;
    }
  `]
})
export class CategoryFormComponent implements OnInit {

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

  submit() {

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
    } else {
      this.categoryService.create(payload)
        .subscribe(() => {
          this.notificationService.success('Category created successfully.');
          this.router.navigate(['/category']);
        });
    }
  }

  cancel(){
    this.router.navigate(['/category']);
  }
}
