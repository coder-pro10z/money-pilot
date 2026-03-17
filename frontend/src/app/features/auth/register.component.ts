import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { AuthFormComponent } from './auth-form.component';
import { NotificationService } from '../../shared/services/notification.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule,AuthFormComponent, RouterModule],
  template: `
<div class="auth-wrapper">

  <app-auth-form
    [title]="'Create Account'"
    [buttonText]="'Register'"
    [form]="form"
    (submit)="register()">

    <p class="auth-footer" style="text-align:center;margin-top:10px;">
      Already have an account?
      <a routerLink="/login">Login</a>
    </p>

  </app-auth-form>

</div>`
})
export class RegisterComponent {

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private notificationService: NotificationService
  ) {}

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  register(){

    if(this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.authService.register(this.form.value)
      .subscribe({
        next: () => {
          this.notificationService.success('Registration successful. You can now sign in.');
          this.router.navigate(['/login']);
        }
      });

  }

}
