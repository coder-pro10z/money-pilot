import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ElementRef, Input, OnChanges, OnDestroy, SimpleChanges, ViewChild } from '@angular/core';
import { Chart } from 'chart.js/auto';
import { DashboardSummary } from '../../../core/models/dashboard.model';

@Component({
  selector: 'app-charts',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="charts-container">
      <div class="chart-card">
        <h3>Monthly Trend</h3>
        <ng-container *ngIf="summary.monthlyTrend.length; else noTrendData">
          <canvas #monthlyChart></canvas>
        </ng-container>
        <ng-template #noTrendData>
          <p class="chart-empty">Add a few months of spending data to see trends here.</p>
        </ng-template>
      </div>

      <div class="chart-card">
        <h3>Category Breakdown</h3>
        <ng-container *ngIf="summary.categoryBreakdown.length; else noCategoryData">
          <canvas #categoryChart></canvas>
        </ng-container>
        <ng-template #noCategoryData>
          <p class="chart-empty">Create expenses in different categories to see a breakdown here.</p>
        </ng-template>
      </div>
    </div>
  `,
  styles: [`
    .charts-container {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
      gap: 20px;
      margin-top: 20px;
      width: 100%;
      max-width: 100%;
    }

    .chart-card {
      background: white;
      padding: 20px;
      border-radius: 10px;
      box-shadow: 0 2px 6px rgba(0,0,0,0.05);
      min-height: 320px;
      width: 100%;
      max-width: 100%;
      overflow: hidden;
      box-sizing: border-box;
    }

    .chart-empty {
      color: #667085;
      margin-top: 24px;
    }

    canvas {
      display: block;
      width: 100% !important;
      max-width: 100%;
      height: auto !important;
    }
  `]
})
export class ChartsComponent implements AfterViewInit, OnChanges, OnDestroy {
  @Input({ required: true }) summary!: DashboardSummary;
  @ViewChild('monthlyChart') monthlyChartRef?: ElementRef<HTMLCanvasElement>;
  @ViewChild('categoryChart') categoryChartRef?: ElementRef<HTMLCanvasElement>;

  private monthlyChart?: Chart;
  private categoryChart?: Chart;

  ngAfterViewInit(): void {
    this.renderCharts();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['summary']) {
      queueMicrotask(() => this.renderCharts());
    }
  }

  ngOnDestroy(): void {
    this.destroyCharts();
  }

  private renderCharts(): void {
    this.destroyCharts();

    if (this.summary.monthlyTrend.length && this.monthlyChartRef) {
      const labels = this.summary.monthlyTrend.map(x => x.month);
      const data = this.summary.monthlyTrend.map(x => x.amount);

      this.monthlyChart = new Chart(this.monthlyChartRef.nativeElement, {
        type: 'line',
        data: {
          labels,
          datasets: [{
            label: 'Monthly Expenses',
            data,
            borderColor: '#4e73df',
            backgroundColor: 'rgba(78,115,223,0.1)',
            fill: true,
            tension: 0.3
          }]
        }
      });
    }

    if (this.summary.categoryBreakdown.length && this.categoryChartRef) {
      const labels = this.summary.categoryBreakdown.map(x => x.category);
      const data = this.summary.categoryBreakdown.map(x => x.amount);

      this.categoryChart = new Chart(this.categoryChartRef.nativeElement, {
        type: 'pie',
        data: {
          labels,
          datasets: [{
            data,
            backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc', '#f6c23e', '#e74a3b']
          }]
        }
      });
    }
  }

  private destroyCharts(): void {
    this.monthlyChart?.destroy();
    this.categoryChart?.destroy();
    this.monthlyChart = undefined;
    this.categoryChart = undefined;
  }
}
