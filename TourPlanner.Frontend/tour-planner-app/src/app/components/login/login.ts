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
  submitting = false;

  constructor(public authService: AuthService) {}

  toggleMode(): void {
    this.mode = this.mode === 'login' ? 'register' : 'login';
    this.errorMessage = '';
  }

  onSubmit(form: any): void {
    if (form.invalid) {
      return;
    }

    this.errorMessage = '';
    this.authService.sessionExpired.set(false);
    this.submitting = true;

    const action = this.mode === 'login'
      ? this.authService.login(this.username, this.password)
      : this.authService.register(this.username, this.password);

    action.subscribe({
      next: () => {
        this.submitting = false;
        /* isLoggedIn-Signal wird im Service automatisch gesetzt */
      },
      error: (err) => {
        this.submitting = false;
        this.errorMessage = err.error?.message || 'Anmeldung fehlgeschlagen';
      }
    });
  }
}