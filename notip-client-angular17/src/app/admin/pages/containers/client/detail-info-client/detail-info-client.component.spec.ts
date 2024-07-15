import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailInfoClientComponent } from './detail-info-client.component';

describe('DetailInfoClientComponent', () => {
  let component: DetailInfoClientComponent;
  let fixture: ComponentFixture<DetailInfoClientComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetailInfoClientComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DetailInfoClientComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
