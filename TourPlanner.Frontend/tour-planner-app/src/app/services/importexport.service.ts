import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ImportExportService {
  private apiUrl = '/api/ImportExport';

  constructor(private http: HttpClient) {}

  exportData(): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/export`, { responseType: 'blob' });
  }

  importData(file: File): Observable<{ importedCount: number }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ importedCount: number }>(`${this.apiUrl}/import`, formData);
  }
}