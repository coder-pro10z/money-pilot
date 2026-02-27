import { Component } from '@angular/core';
import { Router } from '@angular/router';

/**
 * Top navigation bar
 */
@Component({
  selector: 'app-navbar',
  template: `
    <div class="navbar">
      <span>Welcome</span>
      <button (click)="logout()">Logout</button>
    </div>
  `,
  styles: [`
    .navbar {
      display: flex;
      justify-content: space-between;
      padding: 1rem;
      background: #f3f4f6;
      border-bottom: 1px solid #ddd;
    }

    button {
      cursor: pointer;
    }
  `]
})
export class NavbarComponent {

  constructor(private router: Router) {}

  logout() {
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }
}