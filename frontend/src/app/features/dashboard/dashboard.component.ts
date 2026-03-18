import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DashboardSummary } from '../../core/models/dashboard.model';
import { DashboardService } from '../../core/services/dashboard.service';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner.component';
import { ChartsComponent } from './components/charts.component';
import { SummaryCardsComponent } from './components/summary-cards.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, ChartsComponent, LoadingSpinnerComponent, SummaryCardsComponent],
  template: `
    <app-loading-spinner *ngIf="isLoading"></app-loading-spinner>

    <div class="state-card" *ngIf="!isLoading && errorMessage">
      <h3>We couldn't load your dashboard</h3>
      <p>{{ errorMessage }}</p>
      <button class="btn btn-primary" (click)="loadDashboard()">Try Again</button>
    </div>

    <div class="state-card" *ngIf="!isLoading && data && hasNoData">
      <h3>Your dashboard is ready for data</h3>
      <p>Add expenses, budgets, or recurring items to unlock trend charts and insights.</p>
      <div class="state-actions">
        <a class="btn btn-primary" routerLink="/expense/create">Add Expense</a>
        <a class="btn" routerLink="/budget/create">Create Budget</a>
      </div>
    </div>

    <ng-container *ngIf="!isLoading && data && !hasNoData">
      <app-summary-cards [summary]="data"></app-summary-cards>

      <div class="insights-grid">
        <div class="insight-card">
          <h4>Top Spending Category</h4>
          <p>{{ topCategoryLabel }}</p>
        </div>

        <div class="insight-card">
          <h4>Budget Usage</h4>
          <p>{{ budgetUsageLabel }}</p>
        </div>

        <div class="insight-card">
          <h4>Trend Direction</h4>
          <p>{{ trendDirectionLabel }}</p>
        </div>
      </div>

      <app-charts [summary]="data"></app-charts>
    </ng-container>
  `,
  styles: [`
    .state-card {
      background: #ffffff;
      border: 1px dashed #d0d5dd;
      border-radius: 12px;
      padding: 32px 24px;
      text-align: center;
      color: #344054;
    }

    .state-card h3 {
      margin: 0 0 8px;
      color: #101828;
    }

    .state-card p {
      margin: 0 0 16px;
    }

    .state-actions {
      display: flex;
      justify-content: center;
      gap: 12px;
    }

    .insights-grid {
      display: grid;
      grid-template-columns: repeat(3, minmax(0, 1fr));
      gap: 16px;
      margin-bottom: 20px;
    }

    .insight-card {
      background: #fff;
      border-radius: 12px;
      padding: 18px;
      box-shadow: 0 2px 6px rgba(0,0,0,0.05);
    }

    .insight-card h4 {
      margin: 0 0 8px;
      color: #475467;
      font-size: 14px;
    }

    .insight-card p {
      margin: 0;
      color: #101828;
      font-size: 18px;
      font-weight: 600;
    }

    @media (max-width: 768px) {
      .insights-grid {
        grid-template-columns: 1fr;
      }

      .state-actions {
        flex-direction: column;
      }
    }
  `]
})
export class DashboardComponent implements OnInit {
  data: DashboardSummary | null = null;
  isLoading = true;
  errorMessage = '';

  constructor(private dashboardService: DashboardService) {}

  get hasNoData(): boolean {
    if (!this.data) {
      return false;
    }

    return (
      this.data.totalExpenses === 0 &&
      this.data.totalBudget === 0 &&
      this.data.monthlyTrend.length === 0 &&
      this.data.categoryBreakdown.length === 0
    );
  }

  get topCategoryLabel(): string {
    const category = this.data?.categoryBreakdown?.[0];
    return category ? `${category.category} (${category.amount.toLocaleString('en-US', { style: 'currency', currency: 'USD' })})` : 'No category data yet';
  }

  get budgetUsageLabel(): string {
    if (!this.data || this.data.totalBudget <= 0) {
      return 'No budget set yet';
    }

    const percent = Math.min(100, Math.round((this.data.totalExpenses / this.data.totalBudget) * 100));
    return `${percent}% of budget used`;
  }

  get trendDirectionLabel(): string {
    if (!this.data || this.data.monthlyTrend.length < 2) {
      return 'Need more monthly data';
    }

    const latest = this.data.monthlyTrend[this.data.monthlyTrend.length - 1]?.amount ?? 0;
    const previous = this.data.monthlyTrend[this.data.monthlyTrend.length - 2]?.amount ?? 0;

    if (latest > previous) {
      return 'Spending is trending up';
    }

    if (latest < previous) {
      return 'Spending is trending down';
    }

    return 'Spending is stable';
  }

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.dashboardService.getSummary().subscribe({
      next: (res) => {
        this.data = res;
        this.isLoading = false;
      },
      error: () => {
        this.data = null;
        this.errorMessage = 'Please try again in a moment.';
        this.isLoading = false;
      }
    });
  }
}
