import { Component, OnInit } from '@angular/core';
import { ExpenseService } from '../../core/services/expense.service';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-expenses',
  imports: [CommonModule],
  templateUrl: './expenses.component.html'
})
export class ExpensesComponent implements OnInit {
  items: any[] = [];
  constructor(private svc: ExpenseService) {}
  ngOnInit(): void {
    this.svc.list().subscribe((r: any) => (this.items = r || []));
  }
}
