import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TourService } from '../../services/tour.service';
import { AchievementService } from '../../services/achievement.service';
import { Tour } from '../../models/tour.model';
import { TourDetail } from '../tour-detail/tour-detail';
import { TourForm } from '../tour-form/tour-form';
import { ImportExportComponent } from '../import-export/import-export';

@Component({
  selector: 'app-tour-list',
  standalone: true,
  imports: [CommonModule, FormsModule, TourDetail, TourForm, ImportExportComponent],
  templateUrl: './tour-list.html',
  styleUrls: ['./tour-list.scss']
})
export class TourList implements OnInit {
  tours: Tour[] = [];
  selectedTour?: Tour;
  searchText: string = '';
  showForm: boolean = false;

  constructor(private tourService: TourService, private cdr: ChangeDetectorRef, private achievementService: AchievementService) { }

  ngOnInit(): void {
    this.loadTours();
  }

  loadTours(): void {
    this.tourService.getAll().subscribe({
      next: (data) => {
        this.tours = data;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error loading tours:', err)
    });
  }

  onSelect(tour: Tour): void {
    this.selectedTour = tour;
  }

  onSearch(): void {
    if (this.searchText.trim()) {
      this.tourService.search(this.searchText).subscribe({
        next: (data) => {
          this.tours = data;
          this.cdr.detectChanges();
        },
        error: (err) => console.error('Error searching:', err)
      });
    } else {
      this.loadTours();
    }
  }

  onDelete(id: string): void {
    if (confirm('Delete this tour?')) {
      this.tourService.delete(id).subscribe({
        next: () => {
          this.tours = this.tours.filter(t => t.id !== id);
          if (this.selectedTour?.id === id) this.selectedTour = undefined;
          this.cdr.detectChanges();
        },
        error: (err) => console.error('Error deleting:', err)
      });
    }
  }

  onTourCreated(tour: Tour): void {
    this.tours.push(tour);
    this.showForm = false;
    this.achievementService.refresh();
  }

  onTourChanged(): void {
    // Die Tour-Objektreferenz in tours[]/selectedTour wurde bereits von TourDetail
    // gemergt (Object.assign) - hier muessen wir nur noch TourList selbst als
    // "zu pruefen" markieren, damit die Sidebar-Kachel den neuen Stand zeigt.
    this.cdr.detectChanges();
  }

  onImportCompleted(): void {
    this.loadTours();
  }
}