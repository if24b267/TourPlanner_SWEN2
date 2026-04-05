import { Component, Input, Output, EventEmitter, OnInit, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Tour, TourLog } from '../../models/tour.model';
import { TourLogService } from '../../services/tourlog.service';
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

  constructor(private logService: TourLogService) {}

  ngOnInit(): void {
    this.loadLogs();
  }

  ngOnChanges(): void {
    this.loadLogs();
  }

  loadLogs(): void {
    this.logService.getByTourId(this.tour.id!).subscribe({
      next: (data) => this.logs = data,
      error: (err) => console.error('Error loading logs:', err)
    });
  }

  onDelete(): void {
    this.deleted.emit(this.tour.id);
  }

  onLogCreated(): void {
    this.showLogForm = false;
    this.loadLogs();
  }

  onDeleteLog(logId: string): void {
    if (confirm('Delete this log?')) {
      this.logService.delete(this.tour.id!, logId).subscribe({
        next: () => this.loadLogs(),
        error: (err) => console.error('Error deleting log:', err)
      });
    }
  }
}