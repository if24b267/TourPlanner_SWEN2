import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TourImageService {
  private apiUrl = '/api/tours';

  constructor(private http: HttpClient) {}

  upload(tourId: string, file: File): Observable<{ path: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ path: string }>(`${this.apiUrl}/${tourId}/image`, formData);
  }

  getImageUrl(tourId: string): string {
    return `${this.apiUrl}/${tourId}/image?t=${Date.now()}`; // Cache-Busting nach Upload
  }

  delete(tourId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${tourId}/image`);
  }
}