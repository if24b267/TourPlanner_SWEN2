import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrls: ['./login.scss']
})
export class LoginComponent {
  mode: 'login' | 'register' = 'login';
  username = '';
  password = '';
  errorMessage = '';

  constructor(public authService: AuthService) {}

  toggleMode(): void {
    this.mode = this.mode === 'login' ? 'register' : 'login';
    this.errorMessage = '';
  }

  onSubmit(): void {
    this.errorMessage = '';
    this.authService.sessionExpired.set(false);

    const action = this.mode === 'login'
      ? this.authService.login(this.username, this.password)
      : this.authService.register(this.username, this.password);

    action.subscribe({
      next: () => { /* isLoggedIn-Signal wird im Service automatisch gesetzt */ },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Anmeldung fehlgeschlagen';
      }
    });
  }
}