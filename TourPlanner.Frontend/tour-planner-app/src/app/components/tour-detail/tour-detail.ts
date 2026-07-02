import { Component, Input, Output, EventEmitter, OnInit, OnChanges, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Tour, TourLog } from '../../models/tour.model';
import { TourLogService } from '../../services/tourlog.service';
import { TourService } from '../../services/tour.service';
import { TourImageService } from '../../services/tourimage.service';
import { MapPlaceholderComponent } from '../map-placeholder/map-placeholder';
import { TourLogForm } from '../tour-log-form/tour-log-form';

@Component({
  selector: 'app-tour-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, MapPlaceholderComponent, TourLogForm],
  templateUrl: './tour-detail.html',
  styleUrls: ['./tour-detail.scss']
})
export class TourDetail implements OnInit, OnChanges {
  @Input() tour!: Tour;
  @Output() deleted = new EventEmitter<string>();

  logs: TourLog[] = [];
  showLogForm: boolean = false;
  editing: boolean = false;

  imageUrl: string | null = null;
  uploadError: string = '';

  constructor(
    private logService: TourLogService,
    private tourService: TourService,
    private imageService: TourImageService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadLogs();
    this.refreshImage();
  }

  ngOnChanges(): void {
    this.loadLogs();
    this.refreshImage();
  }

  refreshImage(): void {
    this.imageUrl = this.tour.routeImagePath
      ? this.imageService.getImageUrl(this.tour.id!)
      : null;
  }

  loadLogs(): void {
    this.logService.getByTourId(this.tour.id!).subscribe({
      next: (data) => {
        this.logs = data;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error loading logs:', err)
    });
  }

  reloadTour(): void {
    this.tourService.getById(this.tour.id!).subscribe({
      next: (data) => {
        this.tour = data;
        this.refreshImage();
        this.cdr.detectChanges();
      }
    });
  }

  onDelete(): void {
    this.deleted.emit(this.tour.id);
  }

  onLogCreated(): void {
    this.showLogForm = false;
    this.loadLogs();
    this.reloadTour();
  }

  onDeleteLog(logId: string): void {
    if (confirm('Delete this log?')) {
      this.logService.delete(this.tour.id!, logId).subscribe({
        next: () => {
          this.loadLogs();
          this.reloadTour();
        },
        error: (err) => console.error('Error deleting log:', err)
      });
    }
  }

  onImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    const file = input.files[0];
    this.uploadError = '';

    this.imageService.upload(this.tour.id!, file).subscribe({
      next: () => {
        this.tour.routeImagePath = 'uploaded'; // Platzhalter, echter Pfad kommt beim naechsten reload
        this.refreshImage();
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.uploadError = err.error || 'Upload fehlgeschlagen';
      }
    });

    input.value = ''; // erlaubt erneuten Upload derselben Datei
  }

  onImageDelete(): void {
    if (!confirm('Bild löschen?')) return;

    this.imageService.delete(this.tour.id!).subscribe({
      next: () => {
        this.tour.routeImagePath = undefined;
        this.imageUrl = null;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error deleting image:', err)
    });
  }
}