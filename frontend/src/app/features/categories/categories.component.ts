import { Component, OnInit } from '@angular/core';
import { CategoryService } from '../../core/services/category.service';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-categories',
  imports: [CommonModule],
  templateUrl: './categories.component.html'
})
export class CategoriesComponent implements OnInit {
  items: any[] = [];
  constructor(private svc: CategoryService) {}
  ngOnInit(): void {
    this.svc.list().subscribe((r: any) => (this.items = r || []));
  }
}
