import { Component } from '@angular/core';
import { TourList } from './components/tour-list/tour-list';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [TourList],
  template: `<app-tour-list></app-tour-list>`
})
export class App {}