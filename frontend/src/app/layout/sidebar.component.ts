import { CommonModule } from '@angular/common';
import { Component, EventEmitter, HostBinding, Input, Output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, MatIconModule, RouterLink, RouterLinkActive],
  template: `
    <div class="sidebar" [class.collapsed]="collapsed" [class.mobile-open]="isHandset && !collapsed">
      <div class="sidebar-header">
        <h2 *ngIf="!collapsed">MoneyPilot</h2>
        <button class="toggle-btn" (click)="toggle.emit()">
          <mat-icon>menu</mat-icon>
        </button>
      </div>

      <nav class="nav-links">
        <a routerLink="/dashboard" routerLinkActive="active-link" (click)="navigate()">
          <mat-icon>dashboard</mat-icon>
          <span *ngIf="!collapsed" class="label">Dashboard</span>
        </a>
        <a routerLink="/expense" routerLinkActive="active-link" (click)="navigate()">
          <mat-icon>receipt</mat-icon>
          <span *ngIf="!collapsed" class="label">Expenses</span>
        </a>
        <a routerLink="/budget" routerLinkActive="active-link" (click)="navigate()">
          <mat-icon>account_balance_wallet</mat-icon>
          <span *ngIf="!collapsed" class="label">Budgets</span>
        </a>
        <a routerLink="/recurring" routerLinkActive="active-link" (click)="navigate()">
          <mat-icon>repeat</mat-icon>
          <span *ngIf="!collapsed" class="label">Recurring</span>
        </a>
        <a routerLink="/category" routerLinkActive="active-link" (click)="navigate()">
          <mat-icon>category</mat-icon>
          <span *ngIf="!collapsed" class="label">Categories</span>
        </a>
      </nav>
    </div>
  `,
  styles: [`
    .sidebar {
      width: 176px;
      background: #ffffff;
      color: #333;
      border-right: 1px solid #e0e0e0;
      box-shadow: 2px 0 5px rgba(0,0,0,0.05);
      min-height: 100vh;
      transition: width 0.3s ease, transform 0.3s ease;
      overflow-x: hidden;
      display: flex;
      flex-direction: column;
      z-index: 950;
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
      display: flex;
      flex-direction: column;
      padding: 0;
      margin: 0;
    }

    .nav-links a {
      color: #555;
      padding: 0.75rem 1rem;
      display: flex;
      align-items: center;
      cursor: pointer;
      transition: background 0.2s;
      text-decoration: none;
    }

    .nav-links a:hover {
      background: #f5f5f5;
      color: #1f2937;
    }

    .nav-links a mat-icon {
      color: #5f6368;
      margin-right: 12px;
    }

    .nav-links a.active-link {
      background: #eef2ff;
      color: #1d4ed8;
      font-weight: 600;
    }

    .nav-links a.active-link mat-icon {
      color: #1d4ed8;
    }

    .sidebar.collapsed .nav-links a {
      justify-content: center;
      padding: 0.75rem 0;
    }

    .sidebar.collapsed .nav-links a mat-icon {
      margin-right: 0;
    }

    .sidebar.collapsed .sidebar-header {
      justify-content: center;
      padding: 1rem 0;
    }

    .sidebar.collapsed .toggle-btn {
      margin: 0 auto;
    }
    
    .sidebar.collapsed {
      width: 60px;
    }

    @media (max-width: 768px) {
      .sidebar {
        position: fixed;
        top: 0;
        left: 0;
        width: min(240px, 78vw);
        transform: translateX(-100%);
      }

      .sidebar.mobile-open {
        transform: translateX(0);
      }
    }
   `]
})
export class SidebarComponent {
  @Input() collapsed = false;
  @Input() isHandset = false;
  @Output() toggle = new EventEmitter<void>();
  @Output() navigated = new EventEmitter<void>();

  @HostBinding('class.collapsed') get isCollapsed(): boolean {
    return this.collapsed;
  }

  navigate(): void {
    this.navigated.emit();
  }
}
