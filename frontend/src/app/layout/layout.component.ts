import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from './sidebar.component';
import { NavbarComponent } from './navbar.component';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, SidebarComponent, NavbarComponent],
  template: `
    <div class="app-layout">
      <app-sidebar [collapsed]="sidebarCollapsed" (toggle)="toggleSidebar()"></app-sidebar>

      <main class="main-content" [class.sidebar-collapsed]="sidebarCollapsed">
        <!-- Mobile header (only on small screens) -->
        <div class="mobile-header" *ngIf="isHandset">
          <button class="menu-btn" (click)="toggleSidebar()">☰</button>
          <span class="app-title">MoneyPilot</span>
        </div>

        <!-- Navbar (visible on all screens) -->
        <app-navbar></app-navbar>

        <!-- Page content -->
        <router-outlet></router-outlet>
      </main>
    </div>
  `,
  styles: [`
    app-navbar {
      display: block;
      margin-bottom: 20px;
    }

    @media (max-width: 768px) {
      app-navbar {
        margin-top: 10px;
      }
    }

    .app-layout {
      display: flex;
      height: 100vh;
    }

    .main-content {
      flex: 1;
      padding: 20px;
      overflow-y: auto;
      transition: margin-left 0.3s ease;
    }

    /* Desktop styles */
    @media (min-width: 769px) {
      .main-content:not(.sidebar-collapsed) {
        margin-left: 220px;
      }
      .main-content.sidebar-collapsed {
        margin-left: 60px;
      }
      .mobile-header {
        display: none;
      }
    }

    /* Mobile styles */
    @media (max-width: 768px) {
      .app-layout {
        position: relative;
      }

      app-sidebar {
        position: fixed;
        top: 0;
        left: 0;
        height: 100vh;
        z-index: 1000;
        transform: translateX(-100%);
        transition: transform 0.3s ease;
      }

      app-sidebar:not(.collapsed) {
        transform: translateX(0);
      }

      .main-content {
        margin-left: 0 !important;
        width: 100%;
      }

      .mobile-header {
        display: flex;
        align-items: center;
        padding: 10px 16px;
        background: #1f2937;
        color: white;
        margin-bottom: 20px;
        border-radius: 4px;
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
    this.breakpointObserver.observe([Breakpoints.Handset]).subscribe(result => {
      this.isHandset = result.matches;
      this.sidebarCollapsed = this.isHandset; // collapsed = hidden on mobile
    });
  }

  toggleSidebar() {
    this.sidebarCollapsed = !this.sidebarCollapsed;
  }
}