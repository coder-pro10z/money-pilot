import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-login',
  imports: [FormsModule, CommonModule],
  templateUrl: './login.component.html'
})
export class LoginComponent {
  model = { email: '', password: '' };
  constructor(private auth: AuthService, private router: Router) {}

  submit() {
    this.auth.login(this.model).subscribe(() => this.router.navigate(['/dashboard']));
  }
}
