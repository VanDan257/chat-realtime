import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListMessagesComponent } from './list-messages.component';

describe('ListMessagesComponent', () => {
  let component: ListMessagesComponent;
  let fixture: ComponentFixture<ListMessagesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListMessagesComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ListMessagesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
