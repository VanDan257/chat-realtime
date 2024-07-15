import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailInfoStaffComponent } from './detail-info-staff.component';

describe('DetailInfoStaffComponent', () => {
  let component: DetailInfoStaffComponent;
  let fixture: ComponentFixture<DetailInfoStaffComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetailInfoStaffComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DetailInfoStaffComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
