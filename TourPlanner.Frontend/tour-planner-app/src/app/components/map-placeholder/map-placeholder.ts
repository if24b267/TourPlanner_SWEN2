import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-map-placeholder',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './map-placeholder.html',
  styleUrls: ['./map-placeholder.scss']
})
export class MapPlaceholderComponent {
  @Input() height: string = '300px';
  @Input() from: string = '';
  @Input() to: string = '';
}