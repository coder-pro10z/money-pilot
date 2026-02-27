import { Component, OnInit } from '@angular/core';
import { DashboardService } from '../../core/services/dashboard.service';
import { DashboardSummary } from '../../core/models/dashboard.model';

/**
 * DashboardComponent
 *
 * Container component.
 * Fetches dashboard data and passes it to child components.
 */
@Component({
  selector: 'app-dashboard',
  template: `
    <div *ngIf="loading">Loading...</div>

    <ng-container *ngIf="!loading && data">
      <app-summary-cards [summary]="data"></app-summary-cards>
      <app-charts [summary]="data"></app-charts>
    </ng-container>
  `
})
export class DashboardComponent implements OnInit {

  data!: DashboardSummary;
  loading = true;

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.dashboardService.getSummary().subscribe({
      next: (res) => {
        this.data = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }
}