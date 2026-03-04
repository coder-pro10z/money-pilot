import { Component, Input } from '@angular/core';
import { DashboardSummary } from '../../../core/models/dashboard.model';
import { CommonModule } from '@angular/common';

/**
 * Displays top-level financial summary cards.
 */
@Component({
  selector: 'app-summary-cards',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="card-container total-expenses-card">
      <div class="card">
        <h3 class="card-title">Total Expenses</h3>
        <p>{{ summary.totalExpenses | currency }}</p>
      </div>

      <div class="card total-budget-card">
        <h3 class="card-title">Total Budget</h3>
        <p>{{ summary.totalBudget | currency }}</p>
      </div>

      <div class="card rem-budget-card">
        <h3 class="card-title">Remaining Budget</h3>
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
      .card-title {
        margin-bottom: 0.5rem;
        text-shadow: 1px 1px 2px rgba(0,0,0,0.1);
      
        }

    .card {
      padding: 1rem;
      border-radius: 8px;
      background: #f4f4f4;
      flex: 1;
    }

    .card {
      background: #ffebee;
    } 
    .total-budget-card {
      background: #e3f2fd;
    } 
    .rem-budget-card {
      background: #e8f5e9;
    }
  `]
})
export class SummaryCardsComponent {
  @Input() summary!: DashboardSummary;
}