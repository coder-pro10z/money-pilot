import { Component, OnInit } from '@angular/core';
import { DashboardService } from '../../core/services/dashboard.service';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-dashboard',
  imports: [CommonModule],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  data: any;
  constructor(private svc: DashboardService) {}
  ngOnInit(): void {
    this.svc.summary().subscribe((d) => (this.data = d));
  }
}
