import { Component, Input } from '@angular/core';
import { DashboardSummary } from '../../../core/models/dashboard.model';

/**
 * Displays top-level financial summary cards.
 */
@Component({
  selector: 'app-summary-cards',
  template: `
    <div class="card-container">
      <div class="card">
        <h3>Total Expenses</h3>
        <p>{{ summary.totalExpenses | currency }}</p>
      </div>

      <div class="card">
        <h3>Total Budget</h3>
        <p>{{ summary.totalBudget | currency }}</p>
      </div>

      <div class="card">
        <h3>Remaining Budget</h3>
        <p>{{ summary.remainingBalance | currency }}</p>
      </div>
    </div>
  `,
  styles: [`
    .card-container {
      display: flex;
      gap: 1rem;
      margin-bottom: 2rem;
    }

    .card {
      padding: 1rem;
      border-radius: 8px;
      background: #f4f4f4;
      flex: 1;
    }
  `]
})
export class SummaryCardsComponent {
  @Input() summary!: DashboardSummary;
}