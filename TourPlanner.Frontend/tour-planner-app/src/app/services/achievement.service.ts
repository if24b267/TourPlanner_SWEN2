import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Achievement } from '../models/achievement.model';

@Injectable({
  providedIn: 'root'
})
export class AchievementService {
  private apiUrl = '/api/achievements';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Achievement[]> {
    return this.http.get<Achievement[]>(this.apiUrl);
  }
}