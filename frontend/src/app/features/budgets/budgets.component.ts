import { Component, OnInit } from '@angular/core';
import { BudgetService } from '../../core/services/budget.service';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-budgets',
  imports: [CommonModule],
  templateUrl: './budgets.component.html'
})
export class BudgetsComponent implements OnInit {
  items: any[] = [];
  constructor(private svc: BudgetService) {}
  ngOnInit(): void {
    this.svc.list().subscribe((r: any) => (this.items = r || []));
  }
}
