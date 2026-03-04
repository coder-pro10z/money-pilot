import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { CommonModule } from '@angular/common';
import { AuthFormComponent } from './auth-form.component';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  standalone: true,
  selector: 'app-login',
  imports: [ReactiveFormsModule, AuthFormComponent,RouterModule, CommonModule],
  templateUrl: './login.component.html'
})
export class LoginComponent {

  constructor(
    private auth: AuthService,
    private router: Router,
    private fb: FormBuilder
  ) {}

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required]
  });

  submit() {
    if (this.form.invalid) return;

    this.auth.login(this.form.value as { email: string; password: string }).subscribe(() => {
      this.router.navigate(['/dashboard']);
    });
  }
}