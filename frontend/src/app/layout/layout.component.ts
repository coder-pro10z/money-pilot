import { BreakpointObserver } from '@angular/cdk/layout';
import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './navbar.component';
import { SidebarComponent } from './sidebar.component';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, SidebarComponent, NavbarComponent],
  template: `
    <div class="app-layout">
      <div class="sidebar-backdrop" *ngIf="isHandset && !sidebarCollapsed" (click)="closeSidebar()"></div>

      <app-sidebar
        [collapsed]="sidebarCollapsed"
        [isHandset]="isHandset"
        (toggle)="toggleSidebar()"
        (navigated)="handleSidebarNavigation()">
      </app-sidebar>

      <main class="main-content">
        <div class="mobile-header" *ngIf="isHandset">
          <button class="menu-btn" (click)="toggleSidebar()">☰</button>
          <span class="app-title">MoneyPilot</span>
        </div>

        <app-navbar></app-navbar>
        <div class="content-body">
          <router-outlet></router-outlet>
        </div>
      </main>
    </div>
  `,
  styles: [`
    .app-layout {
      display: flex;
      width: 100%;
      min-height: 100vh;
      position: relative;
    }

    .main-content {
      flex: 1;
      min-width: 0;
      overflow-y: auto;
      overflow-x: hidden;
      position: relative;
      z-index: 1;
      display: flex;
      flex-direction: column;
    }

    .content-body {
      padding: 16px;
    }

    .sidebar-backdrop {
      position: fixed;
      inset: 0;
      background: rgba(15, 23, 42, 0.45);
      z-index: 900;
    }

    @media (min-width: 769px) {
      .mobile-header {
        display: none;
      }
    }

    @media (max-width: 768px) {
      .main-content {
        width: 100%;
      }

      .mobile-header {
        display: flex;
        align-items: center;
        padding: 10px 16px;
        background: #1f2937;
        color: white;
        margin-bottom: 20px;
        border-radius: 8px;
      }

      .menu-btn {
        background: transparent;
        border: none;
        color: white;
        font-size: 1.5rem;
        margin-right: 12px;
        cursor: pointer;
        width: 40px;
        height: 40px;
        display: flex;
        align-items: center;
        justify-content: center;
      }

      .app-title {
        font-weight: 500;
        font-size: 1.2rem;
      }
    }
  `]
})
export class LayoutComponent {
  sidebarCollapsed = false;
  isHandset = false;

  constructor(private breakpointObserver: BreakpointObserver) {
    this.breakpointObserver.observe(['(max-width: 768px)']).subscribe(result => {
      this.isHandset = result.matches;
      this.sidebarCollapsed = this.isHandset;
    });
  }

  toggleSidebar(): void {
    this.sidebarCollapsed = !this.sidebarCollapsed;
  }

  closeSidebar(): void {
    if (this.isHandset) {
      this.sidebarCollapsed = true;
    }
  }

  handleSidebarNavigation(): void {
    this.closeSidebar();
  }
}
