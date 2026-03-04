import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="spinner-overlay">
      <div class="spinner"></div>
    </div>
  `,
  styles: [`
    .spinner-overlay {
      display: flex;
      justify-content: center;
      align-items: center;
      padding: 40px 0;
    }

    .spinner {
      width: 40px;
      height: 40px;
      border: 4px solid #e0e0e0;
      border-top: 4px solid #3f51b5;
      border-radius: 50%;
      animation: spin 0.8s linear infinite;
    }

    @keyframes spin {
      to {
        transform: rotate(360deg);
      }
    }
  `]
})
export class LoadingSpinnerComponent {}