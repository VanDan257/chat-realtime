import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ButtonUploadComponent } from './button-upload.component';

describe('ButtonUploadComponent', () => {
  let component: ButtonUploadComponent;
  let fixture: ComponentFixture<ButtonUploadComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ButtonUploadComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ButtonUploadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
