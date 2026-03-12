import { Component, Input, Output, EventEmitter, HostBinding } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  template: `
    <div class="sidebar" [class.collapsed]="collapsed">
      <div class="sidebar-header">
        <h2 *ngIf="!collapsed">MoneyPilot</h2>
        <button class="toggle-btn" (click)="toggle.emit()">
          <mat-icon>menu</mat-icon>
        </button>
      </div>

      <ul class="nav-links">
        <li (click)="navigate('dashboard')">
          <mat-icon>dashboard</mat-icon>
          <span *ngIf="!collapsed" class="label">Dashboard</span>
        </li>
        <li (click)="navigate('expense')">
          <mat-icon>receipt</mat-icon>
          <span *ngIf="!collapsed" class="label">Expenses</span>
        </li>
        <li (click)="navigate('budget')">
          <mat-icon>account_balance_wallet</mat-icon>
          <span *ngIf="!collapsed" class="label">Budgets</span>
        </li>
        <li (click)="navigate('recurring')">
          <mat-icon>repeat</mat-icon>
          <span *ngIf="!collapsed" class="label">Recurring</span>
        </li>
        <li (click)="navigate('category')">
          <mat-icon>category</mat-icon>
          <span *ngIf="!collapsed" class="label">Categories</span>
        </li>
      </ul>
    </div>
  `,
  styles: [`
    .sidebar {
      width: 220px;
      background: #ffffff;
      color: #333;
      border-right: 1px solid #e0e0e0;
      box-shadow: 2px 0 5px rgba(0,0,0,0.05);
      height: 100vh;
      transition: width 0.3s ease;
      overflow-x: hidden;
      display: flex;
      flex-direction: column;
    }

    .sidebar-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 1rem;
      border-bottom: 1px solid #e0e0e0;
      min-height: 64px;
    }

    .sidebar-header h2 {
      color: #1f2937;
      margin: 0;
      font-size: 1.25rem;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    .toggle-btn {
      background: transparent;
      border: none;
      color: #555;
      cursor: pointer;
      width: 40px;
      height: 40px;
      display: flex;
      align-items: center;
      justify-content: center;
      border-radius: 4px;
      flex-shrink: 0;
    }

    .toggle-btn:hover {
      background: #f0f0f0;
    }

    .nav-links {
      list-style: none;
      padding: 0;
      margin: 0;
    }

    .nav-links li {
      color: #555;
      padding: 0.75rem 1rem;
      display: flex;
      align-items: center;
      cursor: pointer;
      transition: background 0.2s;
    }

    .nav-links li:hover {
      background: #f5f5f5;
      color: #1f2937;
    }

    .nav-links li mat-icon {
      color: #5f6368;
      margin-right: 12px;
    }

    .nav-links li:hover mat-icon {
      color: #1f2937;
    }

    /* Collapsed state adjustments */
    .sidebar.collapsed .nav-links li {
      justify-content: center;
      padding: 0.75rem 0;
    }

    .sidebar.collapsed .nav-links li mat-icon {
      margin-right: 0;
    }

    .sidebar.collapsed .sidebar-header {
      justify-content: center;
      padding: 1rem 0;
    }

    .sidebar.collapsed .toggle-btn {
      margin: 0 auto;
    }
  `]
})
export class SidebarComponent {
  @Input() collapsed = false;
  @Output() toggle = new EventEmitter<void>();

  @HostBinding('class.collapsed') get isCollapsed() {
    return this.collapsed;
  }

  constructor(private router: Router) {}

  navigate(path: string) {
    this.router.navigate([path]);
  }
}