import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './shared/header.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent],
  templateUrl: 'app-layout',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'moneypilot-frontend';
}
