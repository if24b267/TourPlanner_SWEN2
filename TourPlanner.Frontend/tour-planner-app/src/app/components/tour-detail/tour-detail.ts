import { Component, Input, Output, EventEmitter, OnInit, OnChanges, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Tour, TourLog } from '../../models/tour.model';
import { TourLogService } from '../../services/tourlog.service';
import { TourService } from '../../services/tour.service';
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

  constructor(
    private logService: TourLogService,
    private tourService: TourService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadLogs();
  }

  ngOnChanges(): void {
    this.loadLogs();
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
}