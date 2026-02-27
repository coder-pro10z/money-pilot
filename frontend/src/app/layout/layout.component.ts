import { Component } from '@angular/core';

/**
 * LayoutComponent
 *
 * Main application shell.
 * Wraps all authenticated routes.
 */
@Component({
  selector: 'app-layout',
  template: `
    <div class="layout">
      <app-sidebar></app-sidebar>

      <div class="main">
        <app-navbar></app-navbar>

        <div class="content">
          <router-outlet/>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .layout {
      display: flex;
      height: 100vh;
    }

    .main {
      flex: 1;
      display: flex;
      flex-direction: column;
    }

    .content {
      padding: 1.5rem;
      overflow-y: auto;
    }
  `]
})
export class LayoutComponent {}