import { Component, Input, Output, EventEmitter, OnInit, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TourLogService } from '../../services/tourlog.service';
import { TourLog } from '../../models/tour.model';

@Component({
  selector: 'app-tour-log-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './tour-log-form.html',
  styleUrls: ['./tour-log-form.scss']
})
export class TourLogForm implements OnInit, OnChanges {
  @Input() tourId!: string;
  @Input() existingLog: TourLog | null = null;
  @Output() logCreated = new EventEmitter<void>();
  @Output() logUpdated = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  log: TourLog = this.blankLog();
  errors: string[] = [];

  constructor(private logService: TourLogService) {}

  ngOnInit(): void {
    this.resetFromInput();
  }

  ngOnChanges(): void {
    this.resetFromInput();
  }

  private blankLog(): TourLog {
    return {
      dateTime: new Date(),
      comment: '',
      difficulty: 5,
      totalDistance: 0,
      totalTimeHours: 0,
      rating: 5
    };
  }

  private resetFromInput(): void {
    this.log = this.existingLog ? { ...this.existingLog } : this.blankLog();
    this.errors = [];
  }

  onSubmit(): void {

    this.errors = [];

    if (this.log.difficulty < 1 || this.log.difficulty > 10)
      this.errors.push('Difficulty must be 1-10');
    if (this.log.rating < 1 || this.log.rating > 10)
      this.errors.push('Rating must be 1-10');
    if (this.log.totalDistance < 0)
      this.errors.push('Distance cannot be negative');
    if (this.log.totalTimeHours < 0)
      this.errors.push('Time cannot be negative');

    if (this.errors.length > 0) return;

    if (this.existingLog) {
      this.logService.update(this.tourId, this.existingLog.id!, this.log).subscribe({
        next: () => this.logUpdated.emit(),
        error: (err) => this.errors.push('Failed to update log: ' + err.message)
      });
    } else {
      this.logService.create(this.tourId, this.log).subscribe({
        next: () => this.logCreated.emit(),
        error: (err) => this.errors.push('Failed to create log: ' + err.message)
      });
    }
  }

  onCancel(): void {
    this.cancelled.emit();
  }
}