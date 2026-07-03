import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TourService } from '../../services/tour.service';
import { Tour, TRANSPORT_TYPES, createBlankTour } from '../../models/tour.model';

@Component({
  selector: 'app-tour-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './tour-form.html',
  styleUrls: ['./tour-form.scss']
})
export class TourForm {
  @Output() tourCreated = new EventEmitter<Tour>();
  
  tour: Tour = createBlankTour();

  transportTypes = TRANSPORT_TYPES;
  errors: string[] = [];

  constructor(private tourService: TourService) {}

  onSubmit(): void {
    this.errors = [];
    
    if (!this.tour.name.trim()) this.errors.push('Name is required');
    if (!this.tour.from.trim()) this.errors.push('From is required');
    if (!this.tour.to.trim()) this.errors.push('To is required');
    
    if (this.errors.length > 0) return;

    this.tourService.create(this.tour).subscribe({
      next: (created) => this.tourCreated.emit(created),
      error: (err) => this.errors.push('Failed to create tour: ' + err.message)
    });
  }
}