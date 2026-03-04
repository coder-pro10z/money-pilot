import { Component, OnInit } from '@angular/core';
import { DashboardService } from '../../core/services/dashboard.service';
import { DashboardSummary } from '../../core/models/dashboard.model';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner.component';
import { CommonModule } from '@angular/common';
import { ChartsComponent } from './components/charts.component';
import { SummaryCardsComponent } from './components/summary-cards.component';
/**
 * DashboardComponent
 *
 * Container component.
 * Fetches dashboard data and passes it to child components.
 */
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports:[CommonModule, ChartsComponent,LoadingSpinnerComponent,SummaryCardsComponent],
  template: `
    
    <app-loading-spinner *ngIf="isLoading"></app-loading-spinner>

    <ng-container *ngIf="!isLoading && data">
      <app-summary-cards [summary]="data"></app-summary-cards>
      <app-charts [summary]="data"></app-charts>
    </ng-container>
  `
})
export class DashboardComponent implements OnInit {

  data!: DashboardSummary;
  isLoading = true;

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.dashboardService.getSummary().subscribe({
      next: (res) => {
        this.data = res;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }
}