import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardComponent } from './dashboard.component';
import { SummaryCardsComponent } from './components/summary-cards.component';
import { ChartsComponent } from './components/charts.component';

@NgModule({
  declarations: [
    DashboardComponent,
    SummaryCardsComponent,
    ChartsComponent
  ],
  imports: [
    CommonModule,
    DashboardRoutingModule
  ]
})
export class DashboardModule {}