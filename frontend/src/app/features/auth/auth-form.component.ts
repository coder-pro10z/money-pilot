import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-auth-form',
  standalone: true,
  templateUrl: './auth-form.component.html',
  styleUrls: ['./auth-form.component.css'],
  imports: [CommonModule, ReactiveFormsModule]
})
export class AuthFormComponent {

  @Input() title!: string;

  @Input() buttonText!: string;

  @Input() form!: FormGroup;

  @Output() submit = new EventEmitter<void>();

}