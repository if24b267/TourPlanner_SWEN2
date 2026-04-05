import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TourLog } from '../models/tour.model';

@Injectable({
  providedIn: 'root'
})
export class TourLogService {
  private apiUrl = '/api/tours';

  constructor(private http: HttpClient) {}

  getByTourId(tourId: string): Observable<TourLog[]> {
    return this.http.get<TourLog[]>(`${this.apiUrl}/${tourId}/logs`);
  }

  create(tourId: string, log: TourLog): Observable<TourLog> {
    return this.http.post<TourLog>(`${this.apiUrl}/${tourId}/logs`, log);
  }

  update(id: string, log: TourLog): Observable<TourLog> {
    // Note: Need tourId for URL, simplified here
    return this.http.put<TourLog>(`${this.apiUrl}/logs/${id}`, log);
  }

  delete(tourId: string, id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${tourId}/logs/${id}`);
  }
}