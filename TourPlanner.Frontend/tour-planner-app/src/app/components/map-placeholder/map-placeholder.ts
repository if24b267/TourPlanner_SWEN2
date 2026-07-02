import { Component, Input, OnChanges, OnDestroy, AfterViewInit, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import * as L from 'leaflet';

@Component({
  selector: 'app-map-placeholder',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './map-placeholder.html',
  styleUrls: ['./map-placeholder.scss']
})
export class MapPlaceholderComponent implements AfterViewInit, OnChanges, OnDestroy {
  @Input() height: string = '300px';
  @Input() from: string = '';
  @Input() to: string = '';
  @Input() routeGeometryJson?: string;

  @ViewChild('mapContainer', { static: true }) mapContainer!: ElementRef<HTMLDivElement>;

  private map?: L.Map;
  private routeLayer?: L.GeoJSON;
  private viewInitialized = false;

  ngAfterViewInit(): void {
    this.viewInitialized = true;
    this.initMap();
    this.renderRoute();
  }

  ngOnChanges(): void {
    if (this.viewInitialized) {
      this.renderRoute();
    }
  }

  ngOnDestroy(): void {
    this.map?.remove();
  }

  private initMap(): void {
    this.map = L.map(this.mapContainer.nativeElement, {
      scrollWheelZoom: false
    }).setView([48.2082, 16.3738], 6); // Wien als Default-Mittelpunkt

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors',
      maxZoom: 18
    }).addTo(this.map);
  }

  private renderRoute(): void {
    if (!this.map) return;

    if (this.routeLayer) {
      this.map.removeLayer(this.routeLayer);
      this.routeLayer = undefined;
    }

    if (!this.routeGeometryJson) return;

    try {
      const geometry = JSON.parse(this.routeGeometryJson);
      this.routeLayer = L.geoJSON(geometry, {
        style: { color: '#1976d2', weight: 4 }
      }).addTo(this.map);

      this.map.fitBounds(this.routeLayer.getBounds(), { padding: [20, 20] });
    } catch (e) {
      console.error('Ungueltiges Routen-GeoJSON:', e);
    }
  }
}