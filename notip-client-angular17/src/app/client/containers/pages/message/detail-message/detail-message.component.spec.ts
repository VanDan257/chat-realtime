import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailMessageComponent } from './detail-message.component';

describe('DetailMessageComponent', () => {
  let component: DetailMessageComponent;
  let fixture: ComponentFixture<DetailMessageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetailMessageComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DetailMessageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
