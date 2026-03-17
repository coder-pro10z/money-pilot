import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

type NotificationKind = 'success' | 'error' | 'info' | 'warning';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly durationMs = 4000;

  constructor(private snackBar: MatSnackBar) {}

  success(message: string): void {
    this.open(message, 'success');
  }

  error(message: string): void {
    this.open(message, 'error');
  }

  info(message: string): void {
    this.open(message, 'info');
  }

  warning(message: string): void {
    this.open(message, 'warning');
  }

  private open(message: string, kind: NotificationKind): void {
    this.snackBar.open(message, 'Dismiss', {
      duration: this.durationMs,
      horizontalPosition: 'right',
      verticalPosition: 'top',
      panelClass: [`app-snackbar`, `app-snackbar-${kind}`]
    });
  }
}
