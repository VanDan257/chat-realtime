import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListMessagesSearchComponent } from './list-messages-search.component';

describe('ListMessagesSearchComponent', () => {
  let component: ListMessagesSearchComponent;
  let fixture: ComponentFixture<ListMessagesSearchComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListMessagesSearchComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ListMessagesSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
