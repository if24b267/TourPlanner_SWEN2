import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ImportExportService } from '../../services/importexport.service';

@Component({
  selector: 'app-import-export',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './import-export.html',
  styleUrls: ['./import-export.scss']
})
export class ImportExportComponent {
  @Output() importCompleted = new EventEmitter<void>();

  statusMessage = '';
  isError = false;

  constructor(private importExportService: ImportExportService) {}

  onExport(): void {
    this.statusMessage = '';
    this.importExportService.exportData().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `tourplanner-export-${new Date().toISOString().slice(0, 10)}.json`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => {
        this.isError = true;
        this.statusMessage = 'Export fehlgeschlagen';
      }
    });
  }

  onImportFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    const file = input.files[0];
    this.statusMessage = '';

    this.importExportService.importData(file).subscribe({
      next: (res) => {
        this.isError = false;
        this.statusMessage = `${res.importedCount} Tour(s) importiert`;
        this.importCompleted.emit();
      },
      error: (err) => {
        this.isError = true;
        this.statusMessage = err.error?.message || 'Import fehlgeschlagen';
      }
    });

    input.value = '';
  }
}