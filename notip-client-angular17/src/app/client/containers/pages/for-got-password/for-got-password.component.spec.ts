import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ForGotPasswordComponent } from './for-got-password.component';

describe('ForGotPasswordComponent', () => {
  let component: ForGotPasswordComponent;
  let fixture: ComponentFixture<ForGotPasswordComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ForGotPasswordComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ForGotPasswordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
