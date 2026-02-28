import { Component } from '@angular/core';
import { Router } from '@angular/router';

/**
 * Sidebar navigation
 */
@Component({
  selector: 'app-sidebar',
    standalone: true,
  template: `
    <div class="sidebar">
      <h2>MoneyPilot</h2>

      <ul>
        <li (click)="navigate('dashboard')">Dashboard</li>
        <li (click)="navigate('expense')">Expenses</li>
        <li (click)="navigate('budgets')">Budgets</li>
        <li (click)="navigate('recurring')">Recurring</li>
      </ul>
    </div>
  `,
  styles: [`
    .sidebar {
      width: 220px;
      background: #1f2937;
      color: white;
      padding: 1rem;
    }

    ul {
      list-style: none;
      padding: 0;
    }

    li {
      padding: 0.5rem 0;
      cursor: pointer;
    }

    li:hover {
      color: #60a5fa;
    }
  `]
})
export class SidebarComponent {

  constructor(private router: Router) {}

  navigate(path: string) {
    this.router.navigate([path]);
  }
}