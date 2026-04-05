import { Component, Input, Output, EventEmitter } from '@angular/core';
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
export class TourLogForm {
  @Input() tourId!: string;
  @Output() logCreated = new EventEmitter<void>();
  
  log: TourLog = {
    id: '',
    dateTime: new Date(),
    comment: '',
    difficulty: 5,
    totalDistance: 0,
    totalTimeHours: 0,
    rating: 5
  };

  errors: string[] = [];

  constructor(private logService: TourLogService) {}

  onSubmit(): void {
  // Debug
  console.log('Sending log:', this.log);
  
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

    this.logService.create(this.tourId, this.log).subscribe({
      next: () => this.logCreated.emit(),
      error: (err) => this.errors.push('Failed to create log: ' + err.message)
    });
  }
}