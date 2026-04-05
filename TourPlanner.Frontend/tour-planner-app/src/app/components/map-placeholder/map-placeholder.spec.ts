import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MapPlaceholderComponent } from './map-placeholder';

describe('MapPlaceholder', () => {
  let component: MapPlaceholderComponent;
  let fixture: ComponentFixture<MapPlaceholderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MapPlaceholderComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(MapPlaceholderComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
