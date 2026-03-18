import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../core/services/auth.service';

/**
 * Top navigation bar
 */
@Component({
  selector: 'app-navbar',
    standalone: true,
  template: `
    <div class="navbar">
      <div class="navbar-left">
        <h2>MoneyPilot</h2>
      </div>

      <div class="navbar-right">
        <span class="welcome-text">Welcome, {{ displayName }}</span>
        <button class="btn btn-danger" (click)="logout()">Logout</button>
      </div>
    </div>
  `,
  styles: [`
    .navbar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      height: 60px;
      width: 100%;
      padding: 0 16px;
      box-sizing: border-box;
      background: #1e293b;
      color: white;
      border-bottom: 1px solid rgba(255,255,255,0.08);
    }

    .navbar-left h2 {
      margin: 0;
      font-size: 1.1rem;
      font-weight: 600;
      color: white;
    }

    .navbar-right {
      display: flex;
      align-items: center;
      gap: 12px;
      min-width: 0;
    }

    .welcome-text {
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    button {
      cursor: pointer;
    }

    @media (max-width: 768px) {
      .navbar {
        padding: 0 12px;
      }

      .welcome-text {
        max-width: 160px;
      }
    }
  `]
})
export class NavbarComponent {
  get displayName(): string {
    const user = this.auth.getCurrentUser();
    return user?.name || user?.email || 'User';
  }

  constructor(private router: Router, private auth: AuthService) {}

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
