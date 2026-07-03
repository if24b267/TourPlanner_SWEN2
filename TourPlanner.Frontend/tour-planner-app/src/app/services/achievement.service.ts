import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Achievement } from '../models/achievement.model';

@Injectable({
  providedIn: 'root'
})
export class AchievementService {
  private apiUrl = '/api/achievements';

  // Zentraler, geteilter Zustand: jede Komponente, die refresh() aufruft
  // (z.B. nach neuer Tour/neuem Log), aktualisiert automatisch alle Stellen,
  // die dieses Signal lesen (z.B. den Zaehler in AchievementsComponent).
  achievements = signal<Achievement[]>([]);

  constructor(private http: HttpClient) {}

  getAll(): Observable<Achievement[]> {
    return this.http.get<Achievement[]>(this.apiUrl);
  }

  refresh(): void {
    this.getAll().subscribe({
      next: (data) => this.achievements.set(data),
      error: (err) => console.error('Error loading achievements:', err)
    });
  }

  get unlockedCount(): number {
    return this.achievements().filter(a => a.unlocked).length;
  }
}