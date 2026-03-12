import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TourLogComponent } from './tour-log.component';

describe('TourLogComponent', () => {
  let component: TourLogComponent;
  let fixture: ComponentFixture<TourLogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TourLogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TourLogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
