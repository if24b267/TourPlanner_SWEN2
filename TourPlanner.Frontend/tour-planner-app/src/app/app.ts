import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TourList } from './components/tour-list/tour-list';
import { LoginComponent } from './components/login/login';
import { AchievementsComponent } from './components/achievements/achievements';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, TourList, LoginComponent, AchievementsComponent],
  template: `
    <app-login *ngIf="!authService.isLoggedIn()"></app-login>

    <ng-container *ngIf="authService.isLoggedIn()">
      <div class="topbar">
        <span class="title">Tour Planner</span>
        <div class="topbar-actions">
          <app-achievements></app-achievements>
          <button (click)="logout()" class="btn-logout">Logout</button>
        </div>
      </div>
      <app-tour-list></app-tour-list>
    </ng-container>
  `,
  styles: [`
    .topbar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 12px 20px;
      background: #1976d2;
      color: white;
    }
    .title {
      font-weight: 600;
      font-size: 18px;
    }
    .topbar-actions {
      display: flex;
      align-items: center;
      gap: 12px;
    }
    .btn-logout {
      background: white;
      color: #1976d2;
      border: none;
      padding: 6px 14px;
      border-radius: 4px;
      cursor: pointer;
    }
  `]
})
export class App {
  constructor(public authService: AuthService) {}

  logout(): void {
    this.authService.logout();
  }
}