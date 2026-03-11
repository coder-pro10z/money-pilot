import { Component, Input, AfterViewInit } from '@angular/core';
import { DashboardSummary } from '../../../core/models/dashboard.model';
import { CommonModule } from '@angular/common';
import { Chart } from 'chart.js/auto';

@Component({
  selector: 'app-charts',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="charts-container">

      <div class="chart-card">
        <h3>Monthly Trend</h3>
        <canvas id="monthlyChart"></canvas>
      </div>

      <div class="chart-card">
        <h3>Category Breakdown</h3>
        <canvas id="categoryChart"></canvas>
      </div>

    </div>
  `,
  styles: [`
    .charts-container{
      display:grid;
      grid-template-columns:1fr 1fr;
      gap:20px;
      margin-top:20px;
    }

    .chart-card{
      background:white;
      padding:20px;
      border-radius:10px;
      box-shadow:0 2px 6px rgba(0,0,0,0.05);
    }
  `]
})
export class ChartsComponent implements AfterViewInit {

  @Input() summary!: DashboardSummary;

  ngAfterViewInit() {
    setTimeout(() => {
      this.createMonthlyChart();
      this.createCategoryChart();
    });
  }

  createMonthlyChart() {

    const labels = this.summary.monthlyTrend.map(x => x.month);
    const data = this.summary.monthlyTrend.map(x => x.amount);

    new Chart("monthlyChart", {
      type: 'line',
      data: {
        labels: labels,
        datasets: [{
          label: 'Monthly Expenses',
          data: data,
          borderColor: '#4e73df',
          backgroundColor: 'rgba(78,115,223,0.1)',
          fill: true
        }]
      }
    });
  }

  createCategoryChart() {

    const labels = this.summary.categoryBreakdown.map(x => x.category);
    const data = this.summary.categoryBreakdown.map(x => x.amount);

    new Chart("categoryChart", {
      type: 'pie',
      data: {
        labels: labels,
        datasets: [{
          data: data,
          backgroundColor: [
            '#4e73df',
            '#1cc88a',
            '#36b9cc',
            '#f6c23e',
            '#e74a3b'
          ]
        }]
      }
    });
  }
}