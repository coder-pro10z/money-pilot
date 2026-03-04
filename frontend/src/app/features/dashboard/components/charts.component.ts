import { Component, Input } from '@angular/core';
import { DashboardSummary } from '../../../core/models/dashboard.model';
import { CommonModule } from '@angular/common';

/**
 * ChartsComponent
 *
 * Displays monthly trend and category breakdown.
 */
@Component({
  selector: 'app-charts',
  standalone: true,
  imports: [CommonModule],
  template: `
    <h3>Monthly Trend</h3>
    <ul>
      <li *ngFor="let item of summary.monthlyTrend">
        {{ item.month }} - {{ item.amount | currency }}
      </li>
    </ul>

    <h3>Category Breakdown</h3>
    <ul>
      <li *ngFor="let item of summary.categoryBreakdown">
        {{ item.category }} - {{ item.amount | currency }}
      </li>
    </ul>
  `
})
export class ChartsComponent {
  @Input() summary!: DashboardSummary;
}