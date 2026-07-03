import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Tour, TourLog, TRANSPORT_TYPES, createBlankTour } from '../../models/tour.model';
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
  editingLog: TourLog | null = null;

  editing: boolean = false;
  editTour: Tour = createBlankTour();
  editErrors: string[] = [];
  transportTypes = TRANSPORT_TYPES;

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

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['tour']) {
      // Der Nutzer hat eine andere Tour ausgewaehlt - jeder offene Bearbeitungszustand
      // bezieht sich noch auf die vorherige Tour und muss verworfen werden, sonst
      // koennte "Save" die neu ausgewaehlte Tour mit den alten Werten ueberschreiben.
      this.editing = false;
      this.editErrors = [];
      this.showLogForm = false;
      this.editingLog = null;
    }
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
        // In die bestehende Instanz mergen statt die Referenz zu ersetzen: tour-list
        // haelt dieselbe Objektreferenz (tours[]/selectedTour) und wuerde eine
        // Neuzuweisung hier nicht mitbekommen, sodass die Karte in der Liste veraltet bliebe.
        Object.assign(this.tour, data);
        this.refreshImage();
        this.cdr.detectChanges();
      }
    });
  }

  onDelete(): void {
    this.deleted.emit(this.tour.id);
  }

  startEdit(): void {
    this.editTour = { ...this.tour };
    this.editErrors = [];
    this.editing = true;
  }

  cancelEdit(): void {
    this.editing = false;
    this.editErrors = [];
  }

  onSaveEdit(): void {
    this.editErrors = [];

    if (!this.editTour.name.trim()) this.editErrors.push('Name is required');
    if (!this.editTour.from.trim()) this.editErrors.push('From is required');
    if (!this.editTour.to.trim()) this.editErrors.push('To is required');

    if (this.editErrors.length > 0) return;

    this.tourService.update(this.tour.id!, this.editTour).subscribe({
      next: (updated) => {
        // Siehe reloadTour(): in die bestehende Referenz mergen, damit tour-list
        // die Aenderung ohne separaten Reload mitbekommt.
        Object.assign(this.tour, updated);
        this.editing = false;
        this.refreshImage();
        this.cdr.detectChanges();
      },
      error: (err) => this.editErrors.push('Failed to update tour: ' + err.message)
    });
  }

  onAddLogClick(): void {
    // Der Button zeigt "Cancel", wann immer das Formular offen ist (egal ob
    // Add- oder Edit-Modus) - also muss er es in diesem Fall auch schliessen,
    // statt es faelschlich in den Add-Modus umzuschalten.
    if (this.showLogForm) {
      this.onLogFormCancelled();
    } else {
      this.editingLog = null;
      this.showLogForm = true;
    }
  }

  onEditLog(log: TourLog): void {
    this.editingLog = log;
    this.showLogForm = true;
  }

  onLogFormCancelled(): void {
    this.showLogForm = false;
    this.editingLog = null;
  }

  onLogSaved(): void {
    this.showLogForm = false;
    this.editingLog = null;
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