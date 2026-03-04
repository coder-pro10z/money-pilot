import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CategoryService } from '../../core/services/category.service';
import { CreateCategoryDto } from '../../core/models/category-create.model';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="form-container">
      <h2>{{ isEdit ? 'Edit Category' : 'Create Category' }}</h2>

      <form [formGroup]="form" (ngSubmit)="submit()">

        <div>
          <label>Name</label>
          <input formControlName="name" type="text" />
        </div>

        <div>
          <label>Description</label>
          <input formControlName="description" type="text" />
        </div>

        <div>
          <label>Color</label>
          <input formControlName="color" type="color" />
        </div>

        <button class="btn" type="submit" [disabled]="form.invalid">Save</button>
        <button class="btn" type="button" (click)="router.navigate(['/category'])">
          Cancel
        </button>
      </form>
    </div>
  `
})
export class CategoryFormComponent implements OnInit {

  form!: FormGroup;
  isEdit = false;
  categoryId!: number;

  constructor(
    private fb: FormBuilder,
    private categoryService: CategoryService,
    private route: ActivatedRoute,
    public router: Router
  ) {}

  ngOnInit(): void {

    this.form = this.fb.group({
      name: ['', Validators.required],
      description: [''],
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

    if (this.form.invalid) return;

    const payload: CreateCategoryDto = this.form.value;

    if (this.isEdit) {
      this.categoryService.update(this.categoryId, payload)
        .subscribe(() => this.router.navigate(['/category']));
    } else {
      this.categoryService.create(payload)
        .subscribe(() => this.router.navigate(['/category']));
    }
  }
}